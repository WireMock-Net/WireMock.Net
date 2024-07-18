// Copyright © WireMock.Net

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

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
        string? passwordOrKey,
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
                // PEM logic based on: https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
#if NET5_0_OR_GREATER
                if (!string.IsNullOrEmpty(passwordOrKey))
                {
                    var certPem = File.ReadAllText(filePath);
                    var cert = X509Certificate2.CreateFromPem(certPem, passwordOrKey);
                    const string defaultPasswordPem = "WireMock.Net";
                    return new X509Certificate2(cert.Export(X509ContentType.Pfx, defaultPasswordPem), defaultPasswordPem);
                }
                return X509Certificate2.CreateFromPemFile(filePath);

#elif NETCOREAPP3_1
                var cert = new X509Certificate2(filePath);
                if (!string.IsNullOrEmpty(passwordOrKey))
                {
                    var key = System.Security.Cryptography.ECDsa.Create()!;
                    key.ImportECPrivateKey(System.Text.Encoding.UTF8.GetBytes(passwordOrKey), out _);
                    return cert.CopyWithPrivateKey(key);
                }
                return cert;
#else                
                throw new InvalidOperationException("Loading a PEM Certificate is only supported for .NET Core App 3.1, .NET 5.0 and higher.");
#endif
            }

            return !string.IsNullOrEmpty(passwordOrKey) ? new X509Certificate2(filePath, passwordOrKey) : new X509Certificate2(filePath);
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