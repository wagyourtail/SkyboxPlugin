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
        public string Path { get; }

        private readonly MyObjectBuilder_EnvironmentDefinition definition;

        public Skybox(MyObjectBuilder_EnvironmentDefinition definition)
        {
            this.definition = definition;
            Path = definition.EnvironmentTexture;
        }

        public override string ToString()
        {
            return "Skybox:'" + (Path ?? "null") + "'";
        }
    }
}
