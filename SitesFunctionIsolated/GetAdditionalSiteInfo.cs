using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PnP.Core.Services;

namespace groveale
{
    public class GetAdditionalSiteInfo
    {
        private readonly ILogger _logger;
        private readonly IPnPContextFactory contextFactory;
        private readonly Settings settings;

        public GetAdditionalSiteInfo(ILoggerFactory loggerFactory, IPnPContextFactory pnpContextFactory)
        {
            _logger = loggerFactory.CreateLogger<GetAdditionalSiteInfo>();
            contextFactory = pnpContextFactory;
            settings = Settings.LoadSettings();
        }

        [Function("GetAdditionalSiteInfo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Parse the url parameters
            NameValueCollection parameters = HttpUtility.ParseQueryString(req.Url.Query);
            var siteUrl = parameters["siteUrl"];

            HttpResponseData response = null;

            try
            {
                using (var pnpContext = await contextFactory.CreateAsync("Default"))
                {
                    var web = await pnpContext.Web.GetAsync(w => w.Title);
                    var site = await pnpContext.Site.GetAsync(s => s.Id, s => s.Url);

                    var siteInfo = new
                    {
                        siteId = site.Id,
                        siteUrl = site.Url,
                        webTitle = web.Title,
                    };

                    response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/json");
                    await response.WriteStringAsync(JsonSerializer.Serialize(siteInfo));
                }
            }
            catch (Exception ex)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(JsonSerializer.Serialize(ex));
            }

            return response;
        }
    }
}
