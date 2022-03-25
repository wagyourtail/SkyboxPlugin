using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avaness.SkyboxPlugin
{
    public class MyGuiScreenSkyboxConfig : MyGuiScreenBase
    {
        public override string GetFriendlyName()
        {
            return "MyGuiScreenSkyboxConfig";
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
        }

        private void CreateControls()
        {
            AddCaption("Skybox Config");

        }
    }
}
