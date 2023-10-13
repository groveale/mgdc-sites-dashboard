using groveale;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PnP.Core.Auth.Services.Builder.Configuration;
using PnP.Core.Services.Builder.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

Settings settings = Settings.LoadSettings();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Add our global configuration instance
        services.AddSingleton(options =>
        {
            var configuration = context.Configuration;
            settings = Settings.LoadSettings();
            configuration.Bind(settings);
            return configuration;
        });

        // Add our configuration class
        services.AddSingleton(options => { return settings; });

        // Add and configure PnP Core SDK
        services.AddPnPCore(options =>
        {
            // Add the base site url
            options.Sites.Add("Default", new PnPCoreSiteOptions
            {
                SiteUrl = settings.SiteUrl
            });
        });

        services.AddPnPCoreAuthentication(options =>
        {
            // Load the certificate to use
            X509Certificate2 cert = CertHelper.GetAppCertificate(settings.Thumbprint);

            // Configure certificate based auth
            options.Credentials.Configurations.Add("CertAuth", new PnPCoreAuthenticationCredentialConfigurationOptions
            {
                ClientId = settings.ClientId,
                TenantId = settings.TenantId,
                X509Certificate = new PnPCoreAuthenticationX509CertificateOptions
                {
                    Certificate = cert
                }
            });

            // Connect this auth method to the configured site
            options.Sites.Add("Default", new PnPCoreAuthenticationSiteOptions
            {
                AuthenticationProviderName = "CertAuth",
            });
        });

    })
    .Build();

host.Run();
