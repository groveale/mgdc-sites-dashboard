using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace groveale
{

    public class SPOHelper
    {
        // This method is no longer needed
        public static long GetSiteRecycleBinSize(ClientContext clientContext)
        {
            var recycleBin = clientContext.Site.RecycleBin;
            clientContext.Load(recycleBin, r => r.IncludeWithDefaultProperties(i => i.Size));
            clientContext.ExecuteQuery();

            long totalSize = 0;
            foreach (var item in recycleBin)
            {
                totalSize += item.Size;
            }

            return totalSize;
        }


        public static ListDetails GetListDetails(ClientContext clientContext, ListDetails listDetail)
        {
            var list = clientContext.Web.Lists.GetByTitle(listDetail.ListName);
            clientContext.Load(list,
                    l => l.Id,
                    l => l.ItemCount,
                    l => l.MajorVersionLimit, 
                    l => l.MajorWithMinorVersionsLimit, 
                    l => l.HasUniqueRoleAssignments, 
                    l => l.NoCrawl
            );
            
            clientContext.ExecuteQuery();

            listDetail.ListId = list.Id.ToString();
            listDetail.ListItemCount = list.ItemCount;
            listDetail.ListMajorVersionCount = list.MajorVersionLimit;
            listDetail.ListMinorVersionCount = list.MajorWithMinorVersionsLimit;
            listDetail.ListHasUniquePermissions = list.HasUniqueRoleAssignments;
            listDetail.IsIndexed = !list.NoCrawl;

            return listDetail;
        }
    }
}