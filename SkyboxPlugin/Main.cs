using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using VRage.Plugins;

namespace avaness.SkyboxPlugin
{
    public class Main : IPlugin
    {
        public static Main Instance;

        public ConfigFile Settings { get; }
        public SkyboxList List { get; }
        public Skybox SelectedSkybox { get; private set; } = Skybox.Default;

        public Main()
        {
            Instance = this;

            Settings = ConfigFile.Load();

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
            if (Settings.SelectedSkybox != 0 && List.TryGetSkybox(Settings.SelectedSkybox, out Skybox selected))
                SelectedSkybox = selected;
        }

        public void Update()
        {

        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyGuiScreenSkyboxConfig());
        }

        public void SetSkybox(Skybox skybox)
        {
            if (skybox.Info == null)
            {
                Settings.SelectedSkybox = 0;
            }
            else
            {
                Settings.SelectedSkybox = skybox.Info.ItemId;
                if (MySession.Static != null && skybox != null)
                    skybox.Load();
            }
            SelectedSkybox = skybox;
            Settings.Save();
        }
    }
}
