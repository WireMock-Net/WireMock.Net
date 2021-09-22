using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Console.Proxy.NETCoreApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = new[] { "http://localhost:9091", "https://localhost:9443" },
                StartAdminInterface = true,
                ReadStaticMappings = false,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "http://postman-echo.com/post",
                    //ClientX509Certificate2ThumbprintOrSubjectName = "www.yourclientcertname.com OR yourcertificatethumbprint (only if the service you're proxying to requires it)",
                    SaveMapping = true,
                    SaveMappingToFile = false,
                    ExcludedHeaders = new[] { "dnt", "Content-Length" }
                },
                Logger= new WireMockConsoleLogger()
            });

            //server.LogEntriesChanged += (sender, eventRecordArgs) =>
            //{
            //    System.Console.WriteLine(JsonConvert.SerializeObject(eventRecordArgs.NewItems, Formatting.Indented));
            //};

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }
    }
}
