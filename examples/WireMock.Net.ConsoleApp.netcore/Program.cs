using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WireMock.Net.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port;
            if (args.Length == 0 || !int.TryParse(args[0], out port))
                port = 8080;

            var server = FluentMockServer.Start(port);
            Console.WriteLine("FluentMockServer running at {0}", server.Port);

            server
              .Given(
                Requests
                    .WithUrl("/*")
                    .UsingGet()
              )
              .RespondWith(
                Responses
                  .WithStatusCode(200)
                  .WithHeader("Content-Type", "application/json")
                  .WithBody(@"{ ""msg"": ""Hello world!""}")
              );

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();


            Console.WriteLine("Displaying all requests");
            var allRequests = server.RequestLogs;
            Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}
