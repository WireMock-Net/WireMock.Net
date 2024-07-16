// Copyright © WireMock.Net

using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.NETCoreApp3WithCertificate
{
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
                    X509StoreName = "My",
                    X509StoreLocation = "CurrentUser",
                    X509StoreThumbprintOrSubjectName = "FE16586076A8B3F3E2F1466803A6C4C7CA35455B"

                    // X509CertificateFilePath = "example.pfx",
                    // X509CertificatePassword = "wiremock"
                }
            });
            System.Console.WriteLine("WireMockServer listening at {0}", string.Join(",", server.Urls));

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }
    }
}