using Sandbox;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System.Linq;
using System.Text;
using VRage.Utils;
using VRageMath;

namespace avaness.SkyboxPlugin
{
    public class MyGuiScreenSkyboxConfig : MyGuiScreenBase
    {
        private Skybox selectedSkybox; 

        public override string GetFriendlyName()
        {
            return "MyGuiScreenSkyboxConfig";
        }

        public MyGuiScreenSkyboxConfig() : base(new Vector2(0.5f), MyGuiConstants.SCREEN_BACKGROUND_COLOR, new Vector2(0.5f, 0.7f), false, null, MySandboxGame.Config.UIBkOpacity, MySandboxGame.Config.UIOpacity)
        {
        }

        public override void UnloadContent()
        {
            if (Main.Instance.SelectedSkybox != selectedSkybox)
                Main.Instance.SelectedSkybox.Load();
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

            selectedSkybox = Main.Instance.SelectedSkybox;

            MyGuiControlListbox listbox = new MyGuiControlListbox();
            listbox.Add(new MyGuiControlListbox.Item(new StringBuilder("None"), userData: Skybox.Default));
            foreach (Skybox skybox in Main.Instance.List.OrderBy(x => x.Info.Title))
            {
                var listItem = new MyGuiControlListbox.Item(new StringBuilder(skybox.Info.Title), userData: skybox);
                listbox.Add(listItem);
                if (skybox == selectedSkybox)
                    listbox.SelectSingleItem(listItem);
            }
            listbox.MultiSelect = false;
            listbox.VisibleRowsCount = 14;
            Controls.Add(listbox);
            listbox.ItemsSelected += Listbox_ItemsSelected;

            CloseButtonEnabled = true;

            MyGuiControlButton closeBtn = new MyGuiControlButton(new Vector2(0, (Size.Value.Y * 0.5f) - (MyGuiConstants.SCREEN_CAPTION_DELTA_Y / 2f)), originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM, text: new StringBuilder("Save"), onButtonClick: OnCloseButtonClick);
            Controls.Add(closeBtn);

        }

        private void OnCloseButtonClick(MyGuiControlButton btn)
        {
            Main.Instance.SetSkybox(selectedSkybox);

            CloseScreen();
        }

        private void Listbox_ItemsSelected(MyGuiControlListbox listbox)
        {
            if (listbox.SelectedItems.Count > 0)
                selectedSkybox = listbox.SelectedItems[0].UserData as Skybox;
            else
                selectedSkybox = Skybox.Default;

            if (MySession.Static != null)
                selectedSkybox.Load();
        }
    }
}
