using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WireMock.HttpsCertificate;

internal static class CertificateLoader
{
    private const string ExtensionPem = ".PEM";

    /// <summary>
    /// Used by the WireMock.Net server
    /// </summary>
    public static X509Certificate2 LoadCertificate(
        string? storeName,
        string? storeLocation,
        string? thumbprintOrSubjectName,
        string? filePath,
        string? password,
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

        if (!string.IsNullOrEmpty(filePath))
        {
            if (filePath!.EndsWith(ExtensionPem, StringComparison.OrdinalIgnoreCase))
            {
#if NET5_0_OR_GREATER
                return !string.IsNullOrEmpty(password) ? X509Certificate2.CreateFromPemFile(filePath, password) : X509Certificate2.CreateFromPemFile(filePath);
#elif NETCOREAPP3_1
                var cert = new X509Certificate2(filePath);
                if (!string.IsNullOrEmpty(password))
                {
                    byte[] passwordAsBytes;
                    try
                    {
                        passwordAsBytes = Convert.FromBase64String(password);
                    }
                    catch
                    {
                        passwordAsBytes = Encoding.UTF8.GetBytes(password);
                    }

                    var key = ECDsa.Create()!;
                    key.ImportECPrivateKey(passwordAsBytes, out _);
                    return cert.CopyWithPrivateKey(key);
                }

                return cert;
#else                
                throw new InvalidOperationException("Loading a PEM CertificateFilePath is only supported for .NETCOREAPP3.1, .NET 5.0 and higher.");
#endif
            }

            return !string.IsNullOrEmpty(password) ? new X509Certificate2(filePath, password) : new X509Certificate2(filePath);
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