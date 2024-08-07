// Copyright © WireMock.Net

using System;
using System.Security.Cryptography.X509Certificates;

namespace WireMock.HttpsCertificate;

/// <summary>
/// Only used for NetStandard 1.3
/// </summary>
internal static class PublicCertificateHelper
{
    // 1] Generate using https://www.pluralsight.com/blog/software-development/selfcert-create-a-self-signed-certificate-interactively-gui-or-programmatically-in-net
    // 2] Converted to Base64
    private const string Data = @"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
";

    public static X509Certificate2 GetX509Certificate2()
    {
        byte[] data = Convert.FromBase64String(Data);
        return new X509Certificate2(data);
    }
}