using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VRage.FileSystem;

namespace avaness.SkyboxPlugin
{
    [XmlRoot("SkyboxConfig")]
    public class ConfigFile
    {
        private static string FileLocation => Path.Combine(MyFileSystem.UserDataPath, "Storage", "SkyboxConfig.xml");

        public ulong SelectedSkybox { get; set; }

        public ConfigFile() { }

        public static ConfigFile Load()
        {
            string file = FileLocation;
            ConfigFile newFile = new ConfigFile();

            if (File.Exists(file))
            {
                try
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ConfigFile));
                    return (ConfigFile)xml.Deserialize(File.OpenRead(file));
                }
                catch { }
            }
            
            newFile.Save();
            return newFile;
        }

        public void Save()
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(typeof(ConfigFile));
                xml.Serialize(File.CreateText(FileLocation), this);
            }
            catch { }
        }
    }
}
