using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WireMock.HttpsCertificate;

internal static class CertificateLoader
{
    /// <summary>
    /// Used by the WireMock.Net server
    /// </summary>
    public static X509Certificate2 LoadCertificate(
        string storeName,
        string storeLocation,
        string thumbprintOrSubjectName,
        string filePath,
        string password,
        string host)
    {
        if (!string.IsNullOrEmpty(storeName) && !string.IsNullOrEmpty(storeLocation))
        {
            var thumbprintOrSubjectNameOrHost = thumbprintOrSubjectName ?? host;

            var certStore = new X509Store((StoreName)Enum.Parse(typeof(StoreName), storeName), (StoreLocation)Enum.Parse(typeof(StoreLocation), storeLocation));
            try
            {
                certStore.Open(OpenFlags.ReadOnly);

                // Attempt to find by Thumbprint first
                var matchingCertificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprintOrSubjectNameOrHost, false);
                if (matchingCertificates.Count == 0)
                {
                    // Fallback to SubjectName
                    matchingCertificates = certStore.Certificates.Find(X509FindType.FindBySubjectName, thumbprintOrSubjectNameOrHost, false);
                    if (matchingCertificates.Count == 0)
                    {
                        // No certificates matched the search criteria.
                        throw new FileNotFoundException($"No Certificate found with in store '{storeName}', location '{storeLocation}' for Thumbprint or SubjectName '{thumbprintOrSubjectNameOrHost}'.");
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

        if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(password))
        {
            return new X509Certificate2(filePath, password);
        }

        if (!string.IsNullOrEmpty(filePath))
        {
            return new X509Certificate2(filePath);
        }

        throw new InvalidOperationException("X509StoreName and X509StoreLocation OR X509CertificateFilePath are mandatory. Note that X509CertificatePassword is optional.");
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

            // Attempt to find by Thumbprint first
            var matchingCertificates = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprintOrSubjectName, false);
            if (matchingCertificates.Count == 0)
            {
                // Fallback to SubjectName
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