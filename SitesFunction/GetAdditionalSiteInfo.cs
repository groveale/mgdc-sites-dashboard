using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace groveale
{
    public static class GetAdditionalSiteInfo
    {
        [FunctionName("GetAdditionalSiteInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string siteId = req.Query["siteId"];
            string siteUrl = req.Query["siteUrl"];
            string primaryAdminId = req.Query["primaryAdminId"];
            string secondaryAdminId = req.Query["secondaryAdminId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            siteId = siteId ?? data?.siteId;
            siteUrl = siteUrl ?? data?.siteUrl;
            primaryAdminId = primaryAdminId ?? data?.primaryAdminId;
            secondaryAdminId = secondaryAdminId ?? data?.secondaryAdminId;

            if (string.IsNullOrEmpty(siteId))
            {
                return new BadRequestObjectResult("Please pass a siteId on the query string or in the request body");
            }

            if (string.IsNullOrEmpty(siteUrl))
            {
                return new BadRequestObjectResult("Please pass a siteUrl on the query string or in the request body");
            }

            // First we need to get all drives for the site
            try 
            {
                // Load settings and initialize GraphHelper with app only auth
                // Method also extracts the required MSGraph data from the spItemURL
                var settings = Settings.LoadSettings();
                
                GraphHelper.InitializeGraphForAppOnlyAuth(settings);

                // We can infer the recycle bin size from the site object
                var siteDetails = await GraphHelper.GetSitesDrives(siteId);

                // Check if the sites owner(s) exist
                siteDetails.IsOrphaned = await GraphHelper.IsSiteOrphaned(siteId, primaryAdminId, secondaryAdminId);

                // Dropping into CSOM (to get list details)
                var spoAuth = new SPOAuthHelper(siteUrl, settings);



                for (int i = 0; i < siteDetails.Lists.Count; i++)
                {
                    var updatedListDetails = SPOHelper.GetListDetails(spoAuth.clientContext, siteDetails.Lists[i]);

                    // Append the list item count to site item count
                    siteDetails.NumberOfItemsInSite += updatedListDetails.ListItemCount; 

                    // Update the list details (not best practice but we are being lazy)
                    siteDetails.Lists[i] = updatedListDetails;
                }

                return new OkObjectResult(siteDetails);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
