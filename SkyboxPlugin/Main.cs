using Sandbox.Graphics.GUI;
using VRage.Plugins;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    public class Main : IPlugin
    {
        public static Main Instance;

        public SkyboxList List { get; }
        public Skybox SelectedSkybox { get; set; }

        public Main()
        {
            Instance = this;
            List = new SkyboxList();
            List.OnListReady += List_OnListReady;
        }

        public void Dispose()
        {
            Instance = null;
        }

        public void Init(object gameInstance)
        {

        }

        private void List_OnListReady()
        {
            MyLog.Default.WriteLine("Skyboxes: ");
            foreach (Skybox skybox in List)
            {
                MyLog.Default.WriteLine(skybox.ToString());
            }
            MyLog.Default.Flush();
        }

        public void Update()
        {

        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyGuiScreenSkyboxConfig());
        }
    }
}
