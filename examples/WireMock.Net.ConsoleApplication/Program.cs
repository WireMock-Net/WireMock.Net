using System;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;


namespace WireMock.Net.ConsoleApplication
{
    static class Program
    {
        static void Main(string[] args)
        {
            int port;
            if (args.Length == 0 || !int.TryParse(args[0], out port))
                port = 8080;

            var server = FluentMockServer.Start(port);
            Console.WriteLine("FluentMockServer running at {0}", server.Port);

            server
                .Given(Request.WithUrl(u => u.Contains("x")).UsingGet())
                .RespondWith(Response
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""/x with FUNC 200""}"));

            // http://localhost:8080/gffgfgf/sddsds?start=1000&stop=1&stop=2
            server
                .Given(Request.WithUrl("/*").UsingGet())
                .RespondWith(Response
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Transformed-Postman-Token", "token is {{request.headers.Postman-Token}}")
                    .WithBody(@"{""msg"": ""Hello world! : {{request.url}} : {{request.path}} : {{request.query.start}} : {{request.query.stop.[0]}}""")
                    .AfterDelay(TimeSpan.FromMilliseconds(100))
                    .WithTransformer()
                );

            server
                .Given(Request.WithUrl("/data").UsingPost().WithBody(b => b.Contains("e")))
                .RespondWith(Response
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""data posted with FUNC 201""}"));

            server
                .Given(Request.WithUrl("/data").UsingPost())
                .RespondWith(Response
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""data posted with 201""}"));

            server
                .Given(Request.WithUrl("/json").UsingPost().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]")))
                .RespondWith(Response
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""json posted with 201""}"));

            server
                .Given(Request.WithUrl("/data").UsingDelete())
                .RespondWith(Response
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""data deleted with 200""}"));

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