using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;

namespace avaness.SkyboxPlugin
{
    public class Skybox
    {
        public string ModName { get; }

        private readonly MyObjectBuilder_EnvironmentDefinition definition;

        public Skybox(Steamworks.SteamUGCDetails_t details, MyObjectBuilder_EnvironmentDefinition definition)
        {
            ModName = details.m_rgchTitle;
            this.definition = definition;
        }

        public override string ToString()
        {
            return "Skybox:'" + ModName + "'";
        }
    }
}
