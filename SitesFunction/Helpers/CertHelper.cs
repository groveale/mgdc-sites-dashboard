
using System;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace groveale
{
    public static class CertHelper
    {
        public static X509Certificate2 GetAppCertificate(string certThumprint)
        {
            X509Certificate2 certificate;
            if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
            {
                certificate = GetAppOnlyCertificate(certThumprint);
            }
            else
            {
                string keyVaultName = Environment.GetEnvironmentVariable("keyVaultName");
                string certNameKV = Environment.GetEnvironmentVariable("certNameKV");
                certificate = GetCertificateFromKV(certNameKV, keyVaultName);
            }

            return certificate;
        }

        private static X509Certificate2 GetCertificateFromKV(string certName, string keyVaultName)
        {
            string secretName = certName; // Name of the certificate created before

            Uri keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

            var client = new SecretClient(keyVaultUri, new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret(secretName);

            return new X509Certificate2(Convert.FromBase64String(secret.Value), string.Empty, X509KeyStorageFlags.MachineKeySet);
        }

        private static X509Certificate2 GetAppOnlyCertificate(string thumbPrint)
        {
            X509Certificate2 appOnlyCertificate = null;
            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, false);
                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                certStore.Close();
                return appOnlyCertificate;
            }
        }
    }
}