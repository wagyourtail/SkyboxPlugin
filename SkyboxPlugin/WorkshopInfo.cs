using Steamworks;
using VRage.Game;

namespace avaness.SkyboxPlugin
{
    public class WorkshopInfo
    {
        // Properties that do not require details
        public ulong ItemId { get; private set; }
        private readonly string modRoot;

        // Properties that require details
        public string Title { get; private set; }
        public MyModContext ModContext { get; private set; }

        public WorkshopInfo(ulong id, string modRoot)
        {
            ItemId = id;
            Title = ItemId.ToString();
            this.modRoot = modRoot;
        }

        public void AddDetails(SteamUGCDetails_t details)
        {
            Title = details.m_rgchTitle;

            MyModContext modContext = new MyModContext();
            modContext.Init(new MyObjectBuilder_Checkpoint.ModItem(ItemId, "Steam"));
            modContext.Init(Title, null, modRoot);
            ModContext = modContext;
        }
    }
}
