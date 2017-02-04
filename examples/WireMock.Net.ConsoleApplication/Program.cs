using System;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.ConsoleApplication
{
    static class Program
    {
        static void Main(params string[] args)
        {
            string url1 = "http://localhost:9090/";
            string url2 = "http://localhost:9091/";
            string url3 = "https://localhost:9443/";

            var server = FluentMockServer.StartWithAdminInterface(url1, url2, url3);
            Console.WriteLine("FluentMockServer listening at {0}", string.Join(" and ", server.Urls));

            server.AllowPartialMapping();

            //server
            //    .Given(Request.Create().WithPath(p => p.Contains("x")).UsingGet())
            //    .AtPriority(4)
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(200)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""Contains x with FUNC 200""}"));

            //server
            //    .Given(Request.Create().WithPath("/data").UsingPost().WithBody(b => b.Contains("e")))
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(201)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""data posted with FUNC 201""}"));

            //server
            //    .Given(Request.Create().WithPath("/data", "/ax").UsingPost().WithHeader("Content-Type", "application/json*"))
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(201)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""data posted with 201""}"));

            //server
            //    .Given(Request.Create().WithPath("/json").UsingPost().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]")))
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(201)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""json posted with 201""}"));

            //server
            //    .Given(Request.Create().WithPath("/json2").UsingPost().WithBody("x"))
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(201)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""json posted with x - 201""}"));

            //server
            //    .Given(Request.Create().WithPath("/data").UsingDelete())
            //    .RespondWith(Response.Create()
            //        .WithStatusCode(200)
            //        .WithHeader("Content-Type", "application/json")
            //        .WithBody(@"{ ""result"": ""data deleted with 200""}"));

            //server
            //    .Given(Request.Create().WithPath("/nobody").UsingGet())
            //    .RespondWith(Response.Create().WithDelay(TimeSpan.FromSeconds(1))
            //        .WithStatusCode(200));

            server
                .Given(Request.Create().WithPath("/partial").UsingGet().WithHeader("p", "p"))
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("partial = 200"));

            // http://localhost:8080/any/any?start=1000&stop=1&stop=2
            server
                .Given(Request.Create().WithPath("/*").UsingGet())
                .WithGuid(Guid.Parse("90356dba-b36c-469a-a17e-669cd84f1f05"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Transformed-Postman-Token", "token is {{request.headers.Postman-Token}}")
                    .WithBody(@"{""msg"": ""Hello world CATCH-ALL on /*, {{request.path}}, bykey={{request.query.start}}, bykey={{request.query.stop}}, byidx0={{request.query.stop.[0]}}, byidx1={{request.query.stop.[1]}}"" }")
                    .WithTransformer()
                    .WithDelay(1000)
                    .WithDelay(TimeSpan.FromMilliseconds(100))
                );

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();

            Console.WriteLine("Displaying all requests");
            var allRequests = server.LogEntries;
            Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}