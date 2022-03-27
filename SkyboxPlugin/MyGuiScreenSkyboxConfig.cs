using Sandbox;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace avaness.SkyboxPlugin
{
    public class MyGuiScreenSkyboxConfig : MyGuiScreenBase
    {
        public override string GetFriendlyName()
        {
            return "MyGuiScreenSkyboxConfig";
        }

        public MyGuiScreenSkyboxConfig() : base(new Vector2(0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.5f, 0.7f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity)
        {

        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            CreateControls();
        }

        private void CreateControls()
        {
            AddCaption("Skybox Config");

            MyGuiControlListbox listbox = new MyGuiControlListbox();
            listbox.Add(new MyGuiControlListbox.Item(new StringBuilder("None"), userData: Skybox.Default));
            foreach (Skybox skybox in Main.Instance.List)
            {
                var listItem = new MyGuiControlListbox.Item(new StringBuilder(skybox.Info.Title), userData: skybox);
                listbox.Add(listItem);
                if (skybox == Main.Instance.SelectedSkybox)
                    listbox.SelectSingleItem(listItem);
            }
            listbox.MultiSelect = false;
            listbox.VisibleRowsCount = 10;
            Controls.Add(listbox);
            listbox.ItemsSelected += Listbox_ItemsSelected;


        }

        private void Listbox_ItemsSelected(MyGuiControlListbox listbox)
        {
            Skybox skybox;
            if (listbox.SelectedItems.Count > 0)
                skybox = listbox.SelectedItems[0].UserData as Skybox;
            else
                skybox = null;

            Main.Instance.SelectedSkybox = skybox;

            if(MySession.Static != null && skybox != null)
                skybox.Load();
        }
    }
}
