using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avaness.SkyboxPlugin
{
    public static class SteamAPI
    {
        public static IEnumerable<ulong> GetSubscribedWorkshopItems()
        {
            uint numItems = SteamUGC.GetNumSubscribedItems();
            if (numItems <= 0)
                return new ulong[0];
            PublishedFileId_t[] ids = new PublishedFileId_t[numItems];
            uint actualNumItems = SteamUGC.GetSubscribedItems(ids, numItems);
            if (actualNumItems < numItems)
                return ids.Where((x, i) => i < actualNumItems).Select(x => x.m_PublishedFileId);
            return ids.Select(x => x.m_PublishedFileId);
        }

        public static void GetItemDetails(Action<Dictionary<ulong, SteamUGCDetails_t>> callback)
        {
            uint numItems = SteamUGC.GetNumSubscribedItems();
            if (numItems > 0)
            {
                PublishedFileId_t[] ids = new PublishedFileId_t[numItems];
                numItems = SteamUGC.GetSubscribedItems(ids, numItems);
                if (numItems == 0)
                    return;

                UGCQueryHandle_t detailsQuery = SteamUGC.CreateQueryUGCDetailsRequest(ids, numItems);
                if (detailsQuery == UGCQueryHandle_t.Invalid)
                    return;

                SteamUGC.AddRequiredTag(detailsQuery, "mod");

                SteamAPICall_t detailsApiCall = SteamUGC.SendQueryUGCRequest(detailsQuery);
                if (detailsApiCall == SteamAPICall_t.Invalid)
                    return;
                var callresult = CallResult<SteamUGCQueryCompleted_t>.Create((x, y) => SteamUGCQueryCompleted(x, y, callback));
                callresult.Set(detailsApiCall);

                SteamUGC.ReleaseQueryUGCRequest(detailsQuery);
            }
        }

        private static void SteamUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure, Action<Dictionary<ulong, SteamUGCDetails_t>> callback)
        {
            if (bIOFailure)
                return;

            Dictionary<ulong, SteamUGCDetails_t> result = new Dictionary<ulong, SteamUGCDetails_t>();
            for (uint i = 0; i < param.m_unNumResultsReturned; i++)
            {
                if (SteamUGC.GetQueryUGCResult(param.m_handle, i, out SteamUGCDetails_t details) && details.m_rgchTags.Contains("mod"))
                    result[details.m_nPublishedFileId.m_PublishedFileId] = details;
            }
            callback(result);
        }
    }
}
