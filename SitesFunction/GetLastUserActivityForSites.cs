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
    public static class LastUserActivity
    {
        [FunctionName("GetLastUserActivityForSites")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            string timePeriod = req.Query["timePeriod"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            timePeriod = timePeriod ?? data?.timePeriod;

            // The main thing time period effects is the active file count for a site
            if (timePeriod != "D7" && timePeriod != "D30" && timePeriod != "D90" && timePeriod != "D180")
            {
                return new BadRequestObjectResult("Please pass a valid timePeriod on the query string or in the request body - Valid options are D7, D30, D90, D180");
            }

            try
            {
                // Load settings and initialize GraphHelper with app only auth
                // Method also extracts the required MSGraph data from the spItemURL
                var settings = Settings.LoadSettings();
                
                GraphHelper.InitializeGraphForAppOnlyAuth(settings);

                var sites = await GraphHelper.GetSiteUserActivityReport(timePeriod);

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
