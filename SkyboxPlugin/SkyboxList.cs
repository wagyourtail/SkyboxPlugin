using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Steamworks;
using VRage.FileSystem;
using VRage.Game;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    public class SkyboxList : IEnumerable<Skybox>
    {
        Dictionary<ulong, Skybox> skyboxes = new Dictionary<ulong, Skybox>();

        public SkyboxList()
        {
            PopulateSkyboxList();
        }

        private void PopulateSkyboxList()
        {
            string workshop = Path.GetFullPath(@"..\..\..\workshop\content\244850\");

            foreach (ulong id in GetSubscribedWorkshopItems())
            {
                string modPath = Path.Combine(workshop, id.ToString());
                if (!Directory.Exists(modPath))
                    continue;

                if (Directory.Exists(Path.Combine(modPath, "Data")))
                {
                    SearchModForSkybox(id, modPath);
                }
                else
                {
                    string legacyFile = Directory.EnumerateFiles(modPath, "*_legacy.bin").FirstOrDefault();
                    if (legacyFile != null)
                        SearchLegacyFileForSkybox(id, legacyFile);
                }
            }
        }

        private void SearchLegacyFileForSkybox(ulong id, string legacyFile)
        {
            foreach (string file in MyFileSystem.GetFiles(legacyFile, "*.sbc", MySearchOption.AllDirectories))
            {
                if (TryGetEnvironmentFile(MyFileSystem.OpenRead(file), out MyObjectBuilder_EnvironmentDefinition def))
                {
                    if (!string.IsNullOrWhiteSpace(def.EnvironmentTexture))
                    {
                        string texture = Path.Combine(legacyFile, def.EnvironmentTexture);
                        if (MyFileSystem.FileExists(texture))
                        {
                            def.EnvironmentTexture = texture;
                            var skybox = new Skybox(def);
                            skyboxes[id] = skybox;
                        }
                    }

                    break;
                }
            }
        }

        private void SearchModForSkybox(ulong id, string modPath)
        {
            string modDataPath = Path.Combine(modPath, "Data");
            foreach (string file in Directory.EnumerateFiles(modDataPath, "*.sbc", SearchOption.AllDirectories))
            {
                if (TryGetEnvironmentFile(File.OpenRead(file), out MyObjectBuilder_EnvironmentDefinition def))
                {
                    if (!string.IsNullOrWhiteSpace(def.EnvironmentTexture))
                    {
                        string texture = Path.Combine(modPath, def.EnvironmentTexture);
                        if(File.Exists(texture))
                        {
                            def.EnvironmentTexture = texture;
                            var skybox = new Skybox(def);
                            skyboxes[id] = skybox;
                        }
                    }

                    break;
                }
            }
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

        private IEnumerable<ulong> GetSubscribedWorkshopItems()
        {
            uint numItems = SteamUGC.GetNumSubscribedItems();
            if (numItems <= 0)
                return new ulong[0];
            PublishedFileId_t[] ids = new PublishedFileId_t[numItems];
            uint actualNumItems = SteamUGC.GetSubscribedItems(ids, numItems);
            if (actualNumItems < numItems)
                return ids.Where((x, i) => i < actualNumItems).Select(x => x.m_PublishedFileId);
            return ids.Select(x => x.m_PublishedFileId);
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
