using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NET6.WithCertificate;

class Program
{
    static void Main(string[] args)
    {
        string url = "https://localhost:8433/";

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Urls = new[] { url },
            StartAdminInterface = true,
            Logger = new WireMockConsoleLogger(),
            CertificateSettings = new WireMockCertificateSettings
            {
                // https://www.scottbrady91.com/c-sharp/pem-loading-in-dotnet-core-and-dotnet
                X509CertificateFilePath = "cert.pem",
                X509CertificatePassword = @"
-----BEGIN EC PRIVATE KEY-----
MHcCAQEEIJZTv6ujGrEwxW+ab1+CtZouRd8PK7PsklVMvJwm1uDmoAoGCCqGSM49
AwEHoUQDQgAE39VoI268uDuIeKmRzr9e9jgMSGeuJTvTG7+cSXmeDymrVgIGXQgm
qKA8TDXpJNrRhWMd/fpsnWu1JwJUjBmspQ==
-----END EC PRIVATE KEY-----"
            }

        });
        System.Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

        System.Console.WriteLine("Press any key to stop the server");
        System.Console.ReadKey();
        server.Stop();
    }
}