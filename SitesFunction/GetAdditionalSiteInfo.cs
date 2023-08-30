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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            siteId = siteId ?? data?.siteId;

            if (string.IsNullOrEmpty(siteId))
            {
                return new BadRequestObjectResult("Please pass a siteId on the query string or in the request body");
            }

            // First we need to get all drives for the site
            try {
                // Load settings and initialize GraphHelper with app only auth
                // Method also extracts the required MSGraph data from the spItemURL
                var settings = Settings.LoadSettings();
                
                GraphHelper.InitializeGraphForAppOnlyAuth(settings);

                var sites = await GraphHelper.GetSitesDrives(siteId);

                return new OkObjectResult(sites);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
