using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Groups.Item.Planner.Plans.Item.Buckets.Item.Tasks;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groveale
{

    class GraphHelper
    {
        
        // Settings object
        private static Settings? _settings;
        // App-ony auth token credential
        private static ClientSecretCredential? _clientSecretCredential;
        // Client configured with app-only authentication
        private static GraphServiceClient? _appClient;


        public static string? _driveId {get;set;}
        public static string? _itemId {get;set;}
        private static string? _parentId {get;set;}
        public static string? _stubId {get;set;}

        // Should really use certificates for this, but this is a demo
        public static void InitializeGraphForAppOnlyAuth(Settings settings)
        {
            _settings = settings;

            // Ensure settings isn't null
            _ = settings ??
                throw new System.NullReferenceException("Settings cannot be null");

            _settings = settings;

            if (_clientSecretCredential == null)
            {
                _clientSecretCredential = new ClientSecretCredential(
                    _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
            }

            if (_appClient == null)
            {
                _appClient = new GraphServiceClient(_clientSecretCredential,
                    // Use the default scope, which will request the scopes
                    // configured on the app registration
                    new[] {"https://graph.microsoft.com/.default"});
            }
        }

        public static async Task<SiteAdditionalDataItem> GetSitesDrives(string siteId)
        {
            // Ensure client isn't null
            _ = _appClient ?? 
                throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

            string[] selectProperties = { "name","driveType","quota","id","webUrl" }; 

            // Do we need to first recurse and get all the subsites?

            var drives = new List<Drive>();
            var result = await _appClient.Sites[siteId].Drives.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = selectProperties;
                // Should really page but we are being lazy
                requestConfiguration.QueryParameters.Top = 999;
            });

            // Initialize the site additional data item 
            var siteAdditionalDataItem = new SiteAdditionalDataItem { SiteId = siteId };

            // Go through the drives and get the size of each
            long totalSize = 0;
            int totalDrives = 0;
            foreach (var drive in result.Value)
            {
                if (drive.WebUrl.EndsWith("PreservationHoldLibrary"))
                {
                    siteAdditionalDataItem.SiteHasPreservationHold = true;
                    siteAdditionalDataItem.StorageUsedPreservationHold = drive.Quota?.Used ?? 0;
                    continue;
                }

                totalDrives++;
                totalSize += drive.Quota?.Used ?? 0;
            }

            siteAdditionalDataItem.NumberOfDrives = totalDrives;
            siteAdditionalDataItem.StorageUsedInDrives = totalSize;

            return siteAdditionalDataItem;
        }

        public static async Task<List<SiteReportItem>> GetSiteUserActivityReport()
        {
            // Ensure client isn't null
            _ = _appClient ??
                throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

            

            // Get the SharePoint site usage detail (D30 is the last 30 days) but will still report on the last activity
            var result = await _appClient.Reports.GetSharePointSiteUsageDetailWithPeriod("D30")
                .GetAsync();

            // result is a CSV file which we convert to a list of objects
            return ConvertCSVStringToObjects(result);
        }

        public static List<SiteReportItem> ConvertCSVStringToObjects(Stream csvStream) 
        {
            var sites = new List<SiteReportItem>();

            bool firstLine = true;

            // Example report data
            //Report Refresh Date,Site Id,Site URL,Owner Display Name,Is Deleted,Last Activity Date,File Count,Active File Count,Page View Count,Visited Page Count,Storage Used (Byte),Storage Allocated (Byte),Root Web Template,Owner Principal Name,Report Period
            //2023-08-28,00ef2338-1d5b-4e42-a60a-ab44c29303ee,https://groverale.sharepoint.com/sites/AllTheDrives,AllTheDrives Owners,False,,2,0,0,0,17683245,109521666048,Group,AllTheDrives@groverale.onmicrosoft.com,30
            using (var reader = new StreamReader(csvStream))
            {
                string line;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        if (firstLine)
                        {
                            firstLine = false;
                            continue;
                        }

                        var x = line.Split(',');
                        
                        sites.Add(new SiteReportItem
                        {
                            // Format is '2023-08-28'
                            ReportRefreshDate = DateTime.Parse(x[0]),
                            SiteId = Guid.Parse(x[1]),
                            SiteURL = x[2],
                            OwnerDisplayName = x[3],
                            IsDeleted = bool.Parse(x[4]),
                            LastActivityDate = x[5] == "" ? null : DateTime.Parse(x[5]),
                            FileCount = int.Parse(x[6]),
                            ActiveFileCount = int.Parse(x[7]),
                            PageViewCount = int.Parse(x[8]),
                            VisitedPageCount = int.Parse(x[9]),
                            StorageUsedBytes = long.Parse(x[10]),
                            StorageAllocatedBytes = long.Parse(x[11]),
                            RootWebTemplate = x[12],
                            OwnerPrincipalName = x[13],
                            ReportPeriod = int.Parse(x[14])
                        });
                    }
                } while (line != null);
            }
        
            return sites;
        }
    }
}