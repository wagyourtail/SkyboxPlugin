using Sandbox.Definitions;
using Sandbox.Game.World;
using VRage.Game;

namespace avaness.SkyboxPlugin
{
    public class Skybox
    {
        public WorkshopInfo Info { get; }

        private readonly MyObjectBuilder_EnvironmentDefinition definition;

        public static Skybox Default { get; } = new Skybox(null, new MyObjectBuilder_EnvironmentDefinition());

        public Skybox(WorkshopInfo workshop, MyObjectBuilder_EnvironmentDefinition definition)
        {
            Info = workshop;
            this.definition = definition;
        }

        public void Load()
        {
            MyObjectBuilder_EnvironmentDefinition ob = definition;
            MyEnvironmentDefinition def = MySector.EnvironmentDefinition;
            if (def == null)
                return;

            def.EnvironmentTexture = ob.EnvironmentTexture;
            def.EnvironmentOrientation = ob.EnvironmentOrientation;
            def.SunProperties = ob.SunProperties;
        }

        public override string ToString()
        {
            return "Skybox:'" + Info.Title + "'";
        }
    }
}
