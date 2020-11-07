using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WireMock.HttpsCertificate
{
    internal static class CertificateLoader
    {
        /// <summary>
        /// Used by the WireMock.Net server
        /// </summary>
        public static X509Certificate2 LoadCertificate(string storeName, string storeLocation, string filePath, string password, string host)
        {
            if (!string.IsNullOrEmpty(storeName) && !string.IsNullOrEmpty(storeLocation))
            {
                var certStore = new X509Store((StoreName)Enum.Parse(typeof(StoreName), storeName), (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation));
                try
                {
                    certStore.Open(OpenFlags.ReadOnly);

                    var certificate = certStore.Certificates.Find(X509FindType.FindBySubjectName, host, validOnly: true);
                    if (certificate.Count == 0)
                    {
                        throw new InvalidOperationException($"No Certificate found with name '{storeName}', location '{storeLocation}' for '{host}'.");
                    }

                    return certificate[0];
                }
                finally
                {
#if NETSTANDARD || NET46
                    certStore.Dispose();
#else
                certStore.Close();
#endif
                }
            }

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(password))
            {
                return new X509Certificate2(filePath, password);
            }

            throw new InvalidOperationException("X509StoreName and X509StoreLocation OR X509CertificateFilePath and X509CertificatePassword are mandatory.");
        }

        /// <summary>
        /// Used for Proxy
        /// </summary>
        public static X509Certificate2 LoadCertificate(string thumbprintOrSubjectName)
        {
            var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                // Certificate must be in the local machine store
                certStore.Open(OpenFlags.ReadOnly);

                // Attempt to find by thumbprint first
                var matchingCertificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprintOrSubjectName, false);
                if (matchingCertificates.Count == 0)
                {
                    // Fallback to subject name
                    matchingCertificates = certStore.Certificates.Find(X509FindType.FindBySubjectName, thumbprintOrSubjectName, false);
                    if (matchingCertificates.Count == 0)
                    {
                        // No certificates matched the search criteria.
                        throw new FileNotFoundException("No certificate found with specified Thumbprint or SubjectName.", thumbprintOrSubjectName);
                    }
                }

                // Use the first matching certificate.
                return matchingCertificates[0];
            }
            finally
            {
#if NETSTANDARD || NET46
                certStore.Dispose();
#else
                certStore.Close();
#endif
            }
        }
    }
}