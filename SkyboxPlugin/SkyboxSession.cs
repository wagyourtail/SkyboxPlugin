using Sandbox.ModAPI;
using System;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace avaness.SkyboxPlugin
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class SkyboxSession : MySessionComponentBase
    {
        public override void BeforeStart()
        {
            Main.Instance.SelectedSkybox?.Load();
        }
    }
}