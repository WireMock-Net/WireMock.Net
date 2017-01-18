# WireMock.Net
A C# .NET version based on https://github.com/alexvictoor/WireMock which tries to mimic the functionality from http://WireMock.org

[![Build status](https://ci.appveyor.com/api/projects/status/b3n6q3ygbww4lyls?svg=true)](https://ci.appveyor.com/project/StefH/wiremock-net)

[![Version](https://img.shields.io/nuget/v/System.Linq.Dynamic.Core.svg)](https://www.nuget.org/packages/System.Linq.Dynamic.Core)

The following frameworks are supported:
- net45

WireMock allows to get an HTTP server in a glance.

A fluent API allows to specify the behavior of the server and hence easily stub and mock webservices and REST ressources.
```csharp
server = FluentMockServer.Start();
server
  .Given(
    Requests.WithUrl("/*")
  )
  .RespondWith(
    Responses
      .WithStatusCode(200)
      .WithBody(@"{ ""msg"": ""Hello world!""}")
  );
```

Based on class HttpListener from the .net framework, it is very lightweight and have no external dependencies. 

# WireMock API in a nutshell

## Start a server
First thing first, to start a server it is as easy as calling a static method, and your done!
```csharp
var server = FluentMockServer.Start();
```
You can pass as an argument a port number but if you do not an available port will be chosen for you. Hence the above line of code start aserver bounded to locahost a random port.
To know on which port your server is listening, just use server property *Port*.

## Configure routes and behaviors
By default the server returns a dummy message with a 404 status code for all requests. To define a route, you need to specify request for this route and the response you want the server to return. This can be done in a fluent way using classes *Requests* and *Responses* as shown in the example below:
```csharp
server
  .Given(
    Requests.WithUrl("/api").UsingGet()
  )
  .RespondWith(
    Responses
      .WithStatusCode(200)
      .WithBody(@"{ ""result"": ""Some data""}")
  ); 
```

The above example is pretty simple. You can be much more selective routing requests according to url patterns, HTTP verbs, request headers and also body contents. Regarding responses, you can specify response headers, status code and body content.
Below a more exhaustive example:
```csharp
server
  .Given(
    Requests
      .WithUrl("/api")
      .UsingPost()
      .WithHeader("X-version", "42")
      .WithBody(@"{ ""msg"": "Hello Body, I will be stripped anyway!!" }");
  )
  .RespondWith(
    Responses
      .WithStatusCode(200)
      .WithHeader("content-type", "application/json")
      .WithBody(@"{ ""result"": ""Some data""}")
  ); 
```

## Verify interactions
The server keeps a log of the received requests. You can use this log to verify the interactions that have been done with the server during a test.  
To get all the request received by the server, you just need to read property *RequestLogs*:
```csharp
var allRequests = server.RequestLogs;
```
If you need to be more specific on the requests that have been send to the server, you can use the very same fluent API that allows to define routes:
```csharp
var customerReadRequests 
  = server.SearchLogsFor(
    Requests
      .WithUrl("/api/customer*")
      .UsingGet()
  ); 
```

# WireMock with your favourite test framework

Obviously you can use your favourite test framework and use WireMock within your tests. In order to avoid flaky tests you should:
  - let WireMock choose dynamicaly ports. It might seem common sens, avoid hard coded ports in your tests!
  - clean up the request log or shutdown the server at the end of each test

Below a simple example using Nunit and NFluent test assertion library:
```csharp
[SetUp]
public void StartMockServer()
{
    _server = FluentMockServer.Start();
}

[Test]
public async void Should_respond_to_request()
{
    // given
    _sut = new SomeComponentDoingHttpCalls();

    _server
        .Given(
            Requests
                .WithUrl("/foo")
                .UsingGet())
        .RespondWith(
            Responses
                .WithStatusCode(200)
                .WithBody(@"{ ""msg"": ""Hello world!"" }")
            );

    // when
    var response 
        = _sut.DoSomething();
    
    // then
    Check.That(response).IsEqualTo(EXPECTED_RESULT);
    // and optionnaly
    Check.That(_server.SearchLogsFor(Requests.WithUrl("/error*")).IsEmpty();
}

...

[TearDown]
public void ShutdownServer()
{
    _server.Stop();
}
```


# WireMock as a standalone process

This is quite straight forward to launch a mock server within a console application. Below a simple "main" method that takes as a parameter from the commandline a port number and then start a mock server that will respond "Hello World" on every request:
```csharp
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

    server
        .Given(Request.WithUrl("/*").UsingGet())
        .RespondWith(Response
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""msg"": ""Hello world!""}")
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
```

# SSL
You can start a standalone mock server listening for HTTPS requests. To do so, there is just a flag to set when creating the server:
```csharp
var server = FluentMockServer.Start(port: 8443, ssl: true);
```
Obviously you need a certificate registered on your box, properly associated with your application and the port number that will be used. This is not really specific to WireMock, not very straightforward and hence the following stackoverflow thread might come handy: [Httplistener with https support](http://stackoverflow.com/questions/11403333/httplistener-with-https-support)

# Simulating delays
A server can be configured with a global delay that will be applied to all requests. To do so you need to call method FluentMockServer.AddRequestProcessingDelay() as below:
```csharp
var server = FluentMockServer.Start();
server.AddRequestProcessingDelay(TimeSpan.FromSeconds(30)); // add a delay of 30s for all requests
```

Delays can also be configured at route level:
```csharp
server
  .Given(
    Requests
      .WithUrl("/slow")
    )
  .RespondWith(
    Responses
      .WithStatusCode(200)
      .WithBody(@"{ ""msg"": ""Hello I'am a little bit slow!"" }")
      .AfterDelay(TimeSpan.FromSeconds(10)
    )
  );
```

# Simulating faults

Currently not done - need to get rid of HttpListener and use lower level TcpListener in order to be able to implement this properly

# Advanced usage

TBD