// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// If https is used, these settings can be used to configure the CertificateSettings in case a custom certificate instead the default .NET certificate should be used.
///
/// X509StoreName and X509StoreLocation should be defined
/// OR
/// X509CertificateFilePath and X509CertificatePassword should be defined
/// </summary>
public class WireMockCertificateSettings
{
    /// <summary>
    /// X509 StoreName (AddressBook, AuthRoot, CertificateAuthority, My, Root, TrustedPeople or TrustedPublisher)
    /// </summary>
    [PublicAPI]
    public string? X509StoreName { get; set; }

    /// <summary>
    /// X509 StoreLocation (CurrentUser or LocalMachine)
    /// </summary>
    [PublicAPI]
    public string? X509StoreLocation { get; set; }

    /// <summary>
    /// X509 Thumbprint or SubjectName (if not defined, the 'host' is used)
    /// </summary>
    [PublicAPI]
    public string? X509StoreThumbprintOrSubjectName { get; set; }

    /// <summary>
    /// X509Certificate FilePath
    /// </summary>
    [PublicAPI]
    public string? X509CertificateFilePath { get; set; }

    /// <summary>
    /// X509Certificate Password
    /// </summary>
    [PublicAPI]
    public string? X509CertificatePassword { get; set; }

    /// <summary>
    /// X509StoreName and X509StoreLocation should be defined
    /// OR
    /// X509CertificateFilePath and X509CertificatePassword should be defined
    /// </summary>
    [PublicAPI]
    public bool IsDefined =>
        !string.IsNullOrEmpty(X509StoreName) && !string.IsNullOrEmpty(X509StoreLocation) ||
        !string.IsNullOrEmpty(X509CertificateFilePath);
}