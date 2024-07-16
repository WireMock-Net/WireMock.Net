// Copyright © WireMock.Net

using System;
using System.Collections.Specialized;
using System.Net.Http;
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

            System.Console.WriteLine("Subscribing to LogEntriesChanged");
            server.LogEntriesChanged += Server_LogEntriesChanged;

            var uri = new Uri(urls[0]);
            var form = new MultipartFormDataContent
            {
                { new StringContent("data"), "test", "test.txt" }
            };
            new HttpClient().PostAsync(uri, form).GetAwaiter().GetResult();

            System.Console.WriteLine("Unsubscribing to LogEntriesChanged");
            server.LogEntriesChanged -= Server_LogEntriesChanged;

            form = new MultipartFormDataContent
            {
                { new StringContent("data2"), "test2", "test2.txt" }
            };
            new HttpClient().PostAsync(uri, form).GetAwaiter().GetResult();

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();
        }

        private static void Server_LogEntriesChanged(object sender, NotifyCollectionChangedEventArgs eventRecordArgs)
        {
            System.Console.WriteLine("Server_LogEntriesChanged : {0}", eventRecordArgs.NewItems.Count);
        }
    }
}