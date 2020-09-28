using System;
using System.Net.Http;
using Newtonsoft.Json;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Proxy.Net452
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] urls = { "http://localhost:9091/", "https://localhost:9443/" };
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = urls,
                StartAdminInterface = true,
                ReadStaticMappings = false,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "http://postman-echo.com/post",
                    //ClientX509Certificate2ThumbprintOrSubjectName = "www.yourclientcertname.com OR yourcertificatethumbprint (only if the service you're proxying to requires it)",
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    ExcludedHeaders = new[] { "dnt", "Content-Length" }
                }
            });

            server.LogEntriesChanged += (sender, eventRecordArgs) =>
            {
                System.Console.WriteLine(JsonConvert.SerializeObject(eventRecordArgs.NewItems, Formatting.Indented));
            };

            var uri = new Uri(urls[0]);
            var form = new MultipartFormDataContent
            {
                { new StringContent("data"), "test", "test.txt" }
            };
            new HttpClient().PostAsync(uri, form).GetAwaiter().GetResult();

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }
    }
}