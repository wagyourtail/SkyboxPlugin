using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using VRage.Game.Components;

namespace avaness.SkyboxPlugin
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class SkyboxSession : MySessionComponentBase
    {
        protected override void UnloadData()
        {
            MyAPIUtilities.Static.MessageEntered -= ChatEntered;
            base.UnloadData();
        }

        public override void BeforeStart()
        {
            MyAPIUtilities.Static.MessageEntered += ChatEntered;
            Main.Instance.SelectedSkybox?.Load();
        }

        private void ChatEntered(string messageText, ref bool sendToOthers)
        {
            if(messageText.Equals("/skybox", System.StringComparison.OrdinalIgnoreCase))
            {
                sendToOthers = false;
                MyGuiSandbox.AddScreen(new MyGuiScreenSkyboxConfig());
            }
        }
    }
}