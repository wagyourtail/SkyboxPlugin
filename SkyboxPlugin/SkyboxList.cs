using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using VRage.FileSystem;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    public class SkyboxList : IEnumerable<Skybox>
    {
        Dictionary<ulong, Skybox> skyboxes = new Dictionary<ulong, Skybox>();

        public event Action OnListReady;

        public SkyboxList()
        {
            PopulateSkyboxList();
        }

        private void PopulateSkyboxList()
        {
            string workshop = Path.GetFullPath(@"..\..\..\workshop\content\244850\");
            Dictionary<ulong, MyObjectBuilder_EnvironmentDefinition> newSkyboxes = new Dictionary<ulong, MyObjectBuilder_EnvironmentDefinition>();

            foreach (ulong id in SteamAPI.GetSubscribedWorkshopItems())
            {
                string modPath = Path.Combine(workshop, id.ToString());
                if (!Directory.Exists(modPath))
                    continue;

                if (Directory.Exists(Path.Combine(modPath, "Data")))
                {
                    if (TryGetModDefinition(modPath, out MyObjectBuilder_EnvironmentDefinition definition))
                        newSkyboxes[id] = definition;
                }
                else
                {
                    string legacyFile = Directory.EnumerateFiles(modPath, "*_legacy.bin").FirstOrDefault();
                    if (legacyFile != null && TryGetLegacyFileDefinition(legacyFile, out MyObjectBuilder_EnvironmentDefinition definition))
                        newSkyboxes[id] = definition;
                }
            }

            SteamAPI.GetItemDetails((x) => OnItemDetailsFound(x, newSkyboxes));
        }

        private void OnItemDetailsFound(Dictionary<ulong, Steamworks.SteamUGCDetails_t> itemDetails, Dictionary<ulong, MyObjectBuilder_EnvironmentDefinition> itemDefinitions)
        {
            foreach(var details in itemDetails.Values)
            {
                ulong key = details.m_nPublishedFileId.m_PublishedFileId;
                if(itemDefinitions.TryGetValue(key, out var itemDefinition))
                {
                    skyboxes[key] = new Skybox(details, itemDefinition);
                }
            }
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
                    return false;
                definition = baseObject.Environments[0];
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
