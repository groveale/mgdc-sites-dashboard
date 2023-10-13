using System.Security.Cryptography.X509Certificates;

namespace groveale
{
    public class AzureFunctionSettings
    {
        public string SharepointTenantUrl { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public StoreName CertificateStoreName { get; set; }
        public StoreLocation CertificateStoreLocation { get; set; }
        public string CertificateThumbprint { get; set; }
    }
}