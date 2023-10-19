using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using PnP.Core.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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


        public static async Task<ListDetails> GetListDetails(ClientContext clientContext, ListDetails listDetail)
        {
            var list = clientContext.Web.Lists.GetByTitle(listDetail.ListName);
            clientContext.Load(list,
                    l => l.Id,
                    l => l.ItemCount,
                    l => l.MajorVersionLimit, 
                    l => l.MajorWithMinorVersionsLimit, 
                    l => l.HasUniqueRoleAssignments, 
                    l => l.NoCrawl,
                    l => l.RootFolder.ServerRelativeUrl
            );

            clientContext.ExecuteQuery();

            // we need the token to use for a REST call 
            var token = clientContext.GetAccessToken();

            var storageMetrics = await GetFolderStorageMetrics(clientContext.Site.Url, list.RootFolder.ServerRelativeUrl, token);

            listDetail.ListId = list.Id.ToString();
            listDetail.ListItemCount = list.ItemCount;
            listDetail.ListMajorVersionCount = list.MajorVersionLimit;
            listDetail.ListMinorVersionCount = list.MajorWithMinorVersionsLimit;
            listDetail.ListHasUniquePermissions = list.HasUniqueRoleAssignments;
            listDetail.IsIndexed = !list.NoCrawl;
            listDetail.ListSizeTotalUsed = storageMetrics != null ? storageMetrics.TotalSize : 0;
            listDetail.ListLastItemModifiedDate = storageMetrics.LastModified;
            listDetail.PreviousVersionsSize = listDetail.ListSizeTotalUsed - listDetail.DriveSizeUsed;

            return listDetail;
        }

        public static async Task<StorageMetrics> GetFolderStorageMetrics(string sharepointUrl, string folderUrl, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                var apiUrl = sharepointUrl.TrimEnd('/') + $"/_api/web/getFolderByServerRelativeUrl('{folderUrl}')?$select=StorageMetrics&$expand=StorageMetrics";

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(responseContent);

                // object casting still neded
                return new StorageMetrics {
                    LastModified = data.d.StorageMetrics.LastModified,
                    TotalFileCount = data.d.StorageMetrics.TotalFileCount,
                    TotalFileStreamSize = data.d.StorageMetrics.TotalFileStreamSize,
                    TotalSize = data.d.StorageMetrics.TotalSize
                };
            }
        }
    }
       
}