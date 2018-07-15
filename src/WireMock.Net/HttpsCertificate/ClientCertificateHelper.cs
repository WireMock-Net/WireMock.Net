using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WireMock.HttpsCertificate
{
    internal static class ClientCertificateHelper
    {
        public static X509Certificate2 GetCertificate(string thumbprintOrSubjectName)
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
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