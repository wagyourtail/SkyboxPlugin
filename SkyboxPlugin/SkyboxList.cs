using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VRage.FileSystem;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    public class SkyboxList : IEnumerable<Skybox>
    {
        Dictionary<ulong, Skybox> skyboxes = new Dictionary<ulong, Skybox>();

        public bool Ready { get; private set; }
        public event Action OnListReady;

        public SkyboxList()
        {
            PopulateSkyboxList();
        }

        public bool TryGetSkybox(ulong id, out Skybox skybox)
        {
            return skyboxes.TryGetValue(id, out skybox);
        }

        private void PopulateSkyboxList()
        {
            string workshop = Path.GetFullPath(@"..\..\..\workshop\content\244850\");

            ConcurrentQueue<Skybox> skyboxes = new ConcurrentQueue<Skybox>();
            Parallel.ForEach(SteamAPI.GetSubscribedWorkshopItems(), id =>
            {
                string modPath = Path.Combine(workshop, id.ToString());
                if (!Directory.Exists(modPath))
                    return;

                if (Directory.Exists(Path.Combine(modPath, "Data")))
                {
                    if (TryGetModDefinition(modPath, out MyObjectBuilder_EnvironmentDefinition definition))
                        skyboxes.Enqueue(new Skybox(new WorkshopInfo(id, modPath), definition));
                }
                else
                {
                    string legacyFile = Directory.EnumerateFiles(modPath, "*_legacy.bin").FirstOrDefault();
                    if (legacyFile != null && TryGetLegacyFileDefinition(legacyFile, out MyObjectBuilder_EnvironmentDefinition definition))
                        skyboxes.Enqueue(new Skybox(new WorkshopInfo(id, legacyFile), definition));
                }
            });

            this.skyboxes = skyboxes.ToDictionary(x => x.Info.ItemId);
            SteamAPI.GetItemDetails(this.skyboxes.Keys, OnItemDetailsFound);
        }

        private void OnItemDetailsFound(Dictionary<ulong, Steamworks.SteamUGCDetails_t> itemDetails)
        {
            foreach(Skybox skybox in skyboxes.Values)
            {
                WorkshopInfo info = skybox.Info;
                if (itemDetails.TryGetValue(info.ItemId, out Steamworks.SteamUGCDetails_t details))
                    info.AddDetails(details);
                else
                    MyLog.Default.WriteLine("Failed to add details to " + info.ItemId);
            }

            Ready = true;
            if (OnListReady != null)
                OnListReady.Invoke();
        }

        private bool TryGetLegacyFileDefinition(string legacyFile, out MyObjectBuilder_EnvironmentDefinition definition)
        {
            definition = null;
            foreach (string file in MyFileSystem.GetFiles(legacyFile, "*.sbc", MySearchOption.AllDirectories))
            {
                if (TryGetEnvironmentFile(MyFileSystem.OpenRead(file), out MyObjectBuilder_EnvironmentDefinition fileDefinition))
                {
                    if (!string.IsNullOrWhiteSpace(fileDefinition.EnvironmentTexture))
                    {
                        string texture = Path.Combine(legacyFile, fileDefinition.EnvironmentTexture);
                        if (MyFileSystem.FileExists(texture))
                        {
                            fileDefinition.EnvironmentTexture = texture;
                            definition = fileDefinition;
                            return true;
                        }
                    }

                    break;
                }
            }
            return false;
        }

        private bool TryGetModDefinition(string modPath, out MyObjectBuilder_EnvironmentDefinition definition)
        {
            definition = null;
            string modDataPath = Path.Combine(modPath, "Data");
            foreach (string file in Directory.EnumerateFiles(modDataPath, "*.sbc", SearchOption.AllDirectories))
            {
                if (TryGetEnvironmentFile(File.OpenRead(file), out MyObjectBuilder_EnvironmentDefinition fileDefinition))
                {
                    if (!string.IsNullOrWhiteSpace(fileDefinition.EnvironmentTexture))
                    {
                        string texture = Path.Combine(modPath, fileDefinition.EnvironmentTexture);
                        if(File.Exists(texture))
                        {
                            fileDefinition.EnvironmentTexture = texture;
                            definition = fileDefinition;
                            return true;
                        }
                    }

                    break;
                }
            }
            return false;
        }

        private bool TryGetEnvironmentFile(Stream stream, out MyObjectBuilder_EnvironmentDefinition definition)
        {
            definition = null;

            bool isEnvFile = false;
            {
                StreamReader reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("<Environment>"))
                    {
                        isEnvFile = true;
                        break;
                    }
                    else if (line.Contains("xsi:type="))
                    {
                        if (line.Contains("xsi:type=\"EnvironmentDefinition\""))
                        {
                            isEnvFile = true;
                            break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (!isEnvFile)
                return false;

            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                MyObjectBuilder_Definitions baseObject;
                if (!MyObjectBuilderSerializer.DeserializeXML(stream, out baseObject) || baseObject == null)
                    return false;

                if (baseObject.Environments == null || baseObject.Environments.Length == 0)
                {
                    foreach(MyObjectBuilder_DefinitionBase def in baseObject.Definitions)
                    {
                        if(def is MyObjectBuilder_EnvironmentDefinition envDef)
                        {
                            definition = envDef;
                            break;
                        }
                    }
                }
                else
                {
                    definition = baseObject.Environments[0];
                }
            }
            catch
            {
                return false;
            }

            return definition != null;
        }

        public IEnumerator<Skybox> GetEnumerator()
        {
            return skyboxes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return skyboxes.Values.GetEnumerator();
        }
    }
}
