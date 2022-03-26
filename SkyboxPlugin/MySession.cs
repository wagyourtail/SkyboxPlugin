using Sandbox.ModAPI;
using System;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class MySession : MySessionComponentBase
    {

        public override void LoadData()
        {
            Main.Instance.SelectedSkybox?.Load();
        }
    }
}