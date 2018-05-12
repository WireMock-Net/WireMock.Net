using System;
using System.Net;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.ConsoleApplication
{
    public static class MainApp
    {
        public static void Run()
        {
            string url1 = "http://localhost:9091/";
            string url2 = "http://localhost:9092/";
            string url3 = "https://localhost:9443/";

            var server = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { url1, url2, url3 },
                StartAdminInterface = true,
                ReadStaticMappings = true,
                WatchStaticMappings = true,
                //ProxyAndRecordSettings = new ProxyAndRecordSettings
                //{
                //    SaveMapping = true
                //},
                PreWireMockMiddlewareInit = app => { System.Console.WriteLine($"PreWireMockMiddlewareInit : {app.GetType()}"); },
                PostWireMockMiddlewareInit = app => { System.Console.WriteLine($"PostWireMockMiddlewareInit : {app.GetType()}"); }
            });
            System.Console.WriteLine("FluentMockServer listening at {0}", string.Join(",", server.Urls));

            server.SetBasicAuthentication("a", "b");

            server.AllowPartialMapping();

            // .WithHeader("Stef", "Stef")
            //server
            //    .Given(Request.Create().WithPath("*"))
            //    .RespondWith(Response.Create()
            //    .WithProxy("http://restcountries.eu"));

            server
                .Given(Request
                    .Create()
                    .WithPath("/jsonthings")
                    .WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"))
                    .UsingPut())
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""result"": ""JsonPathMatcher !!!""}"));

            server
                .Given(Request
                    .Create()
                    .WithPath(new WildcardMatcher("/navision/OData/Company('My Company')/School*", true))
                    .WithParam("$filter", "(substringof(Code, 'WA')")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithBody(@"{ ""result"": ""odata""}"));

            server
                .Given(Request.Create().WithPath("/headers", "/headers_test").UsingPost().WithHeader("Content-Type", "application/json*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    //.WithHeader("MyHeader", "application/json", "application/json2")
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "data:headers posted with 201" }));

            server
                .Given(Request.Create().WithPath("/file").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile(@"c:\temp\x.json", false)
                );

            server
                .Given(Request.Create().WithPath("/filecache").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile(@"c:\temp\x.json")
                );

            server
                .Given(Request.Create().WithPath("/file_rel").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyFromFile("Program.cs", false)
                );

            server
                .Given(Request.Create().WithHeader("ProxyThis", "true")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithProxy("http://www.google.com")
            );

            server
                .Given(Request.Create().WithPath("/bodyasbytes.png")
                .UsingGet())
                .RespondWith(Response.Create()
                    .WithHeader("Content-Type", "image/png")
                    .WithBody(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAIAAAACUFjqAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTczbp9jAAAAJ0lEQVQoU2NgUPuPD6Hz0RCEAtJoiAxpCCBXGgmRIo0TofORkdp/AMiMdRVnV6O0AAAAAElFTkSuQmCC"))
                );

            server
                .Given(Request.Create().WithPath("/oauth2/access").UsingPost().WithBody("grant_type=password;username=u;password=p"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { access_token = "AT", refresh_token = "RT" }));

            server
                .Given(Request.Create().WithPath("/helloworld").UsingGet().WithHeader("Authorization", new RegexMatcher("^(?i)Bearer AT$")))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("hi"));

            server
                .Given(Request.Create().WithPath(p => p.Contains("x")).UsingGet())
                .AtPriority(4)
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""Contains x with FUNC 200""}"));

            server
                .Given(Request.Create().WithPath("/data").UsingPost().WithBody(b => b.Contains("e")))
                .AtPriority(999)
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBodyAsJson(new { result = "data posted with FUNC 201" }));

            server
                .Given(Request.Create().WithPath("/json").UsingPost().WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]")))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""json posted with 201""}"));

            server
                .Given(Request.Create().WithPath("/json2").UsingPost().WithBody("x"))
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""json posted with x - 201""}"));

            server
                .Given(Request.Create().WithPath("/data").UsingDelete())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{ ""result"": ""data deleted with 200""}"));

            server
                .Given(Request.Create()
                    .WithPath("/needs-a-key")
                    .UsingGet()
                    .WithHeader("api-key", "*", MatchBehaviour.AcceptOnMatch)
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBody(@"{ ""result"": ""api-key found""}"));

            server
                .Given(Request.Create()
                    .WithPath("/needs-a-key")
                    .UsingGet()
                    .WithHeader("api-key", "*", MatchBehaviour.RejectOnMatch)
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.Unauthorized)
                    .WithBody(@"{ ""result"": ""api-key missing""}"));



            server
                .Given(Request.Create().WithPath("/nobody").UsingGet())
                .RespondWith(Response.Create().WithDelay(TimeSpan.FromSeconds(1))
                    .WithStatusCode(200));

            server
                .Given(Request.Create().WithPath("/partial").UsingPost().WithBody(new SimMetricsMatcher(new[] { "cat", "dog" })))
                .RespondWith(Response.Create().WithStatusCode(200).WithBody("partial = 200"));

            // http://localhost:8080/any/any?start=1000&stop=1&stop=2
            server
                .Given(Request.Create().WithPath("/trans").UsingGet())
                .WithGuid("90356dba-b36c-469a-a17e-669cd84f1f05")
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Transformed-Postman-Token", "token is {{request.headers.Postman-Token}}")
                    .WithBody(@"{""msg"": ""Hello world CATCH-ALL on /*, {{request.path}}, bykey={{request.query.start}}, bykey={{request.query.stop}}, byidx0={{request.query.stop.[0]}}, byidx1={{request.query.stop.[1]}}"" }")
                    .WithTransformer()
                    .WithDelay(TimeSpan.FromMilliseconds(100))
                );

            server
                .Given(Request.Create()
                    .WithPath("/state1")
                    .UsingGet())
                .InScenario("s1")
                .WillSetStateTo("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 1"));

            server
                .Given(Request.Create()
                    .WithPath("/foostate1")
                    .UsingGet())
                .InScenario("s1")
                .WhenStateIs("Test state 1")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 1"));

            server
                .Given(Request.Create()
                    .WithPath("/state2")
                    .UsingGet())
                .InScenario("s2")
                .WillSetStateTo("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("No state msg 2"));

            server
                .Given(Request.Create()
                    .WithPath("/foostate2")
                    .UsingGet())
                .InScenario("s2")
                .WhenStateIs("Test state 2")
                .RespondWith(Response.Create()
                    .WithBody("Test state msg 2"));

            System.Console.WriteLine("Press any key to stop the server");
            System.Console.ReadKey();
            server.Stop();

            System.Console.WriteLine("Displaying all requests");
            var allRequests = server.LogEntries;
            System.Console.WriteLine(JsonConvert.SerializeObject(allRequests, Formatting.Indented));

            System.Console.WriteLine("Press any key to quit");
            System.Console.ReadKey();
        }
    }
}