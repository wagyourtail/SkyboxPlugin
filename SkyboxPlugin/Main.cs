using VRage.Plugins;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    public class Main : IPlugin
    {
        SkyboxList list;

        public void Dispose()
        {

        }

        public void Init(object gameInstance)
        {
            list = new SkyboxList();

            MyLog.Default.WriteLine("Skyboxes: ");
            foreach(Skybox skybox in list)
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

        }
    }
}
