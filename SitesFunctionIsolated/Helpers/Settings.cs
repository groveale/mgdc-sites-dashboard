using System;

namespace groveale
{
    public class Settings
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? TenantId { get; set; }
        public string? Thumbprint { get; set; }
        public string? SiteUrl { get; set; }
        public static Settings LoadSettings()
        {
            return new Settings 
            {
                ClientId = Environment.GetEnvironmentVariable("clientId"),
                ClientSecret = Environment.GetEnvironmentVariable("clientSecret"),
                TenantId = Environment.GetEnvironmentVariable("tenantId"),
                Thumbprint = Environment.GetEnvironmentVariable("thumbprint"),
                SiteUrl = Environment.GetEnvironmentVariable("sharepointTenantUrl"),
            };
        }
    }
}