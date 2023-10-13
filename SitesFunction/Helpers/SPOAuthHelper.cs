using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using PnP.Framework;

namespace groveale
{
    public class SPOAuthHelper
    {
        public ClientContext clientContext {get;set;}

        public SPOAuthHelper(string siteUrl, Settings settings)
        {
            clientContext = new PnP.Framework.AuthenticationManager(
                settings.ClientId, CertHelper.GetAppCertificate(settings.Thumbprint), settings.TenantId, null, PnP.Framework.AzureEnvironment.Production, null
                )
                .GetContext($"{siteUrl}");

            // Load the site url to check we have access
            clientContext.Load(clientContext.Site, s => s.Url);
            clientContext.ExecuteQuery();

            Console.WriteLine($"Connected to {clientContext.Site.Url} using SPO Auth");
        }

        
    }
}