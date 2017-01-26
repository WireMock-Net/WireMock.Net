# WireMock.Net
A C# .NET version based on https://github.com/alexvictoor/WireMock which tries to mimic the functionality from http://WireMock.org

[![Build status](https://ci.appveyor.com/api/projects/status/b3n6q3ygbww4lyls?svg=true)](https://ci.appveyor.com/project/StefH/wiremock-net)

[![Version](https://img.shields.io/nuget/v/WireMock.Net.svg)](https://www.nuget.org/packages/WireMock.Net)

Based on class HttpListener from the .net framework, it is very lightweight and have no external dependencies. 

## Stubbing
A core feature of WireMock is the ability to return canned HTTP responses for requests matching criteria.

### Start a server
First thing first, to start a server it is as easy as calling a static method, and your done!
```csharp
var server = FluentMockServer.Start();
```
You can pass as an argument a port number but if you do not an available port will be chosen for you. Hence the above line of code start aserver bounded to locahost a random port.
To know on which port your server is listening, just use server property *Port*.

### Basic stubbing
The following code will configure a response with a status of 200 to be returned when the relative URL exactly matches /some/thing (including query parameters). The body of the response will be “Hello world!” and a Content-Type header will be sent with a value of text-plain.

```csharp
var server = FluentMockServer.Start();
server
  .Given(
    Request.Create().WithUrl("/some/thing").UsingGet()
  )
  .RespondWith(
    Response.Create()
      .WithStatusCode(200)
      .WithHeader("Content-Type", "text/plain")
      .WithBody("Hello world!")
  );
```
HTTP methods currently supported are: GET, POST, PUT, DELETE, HEAD. You can specify ANY (`.UsingAny`) if you want the stub mapping to match on any request method.

A response body in binary format can be specified as a `byte[]` via an overloaded `WithBody()`:

```csharp
var server = FluentMockServer.Start();
server
  .Given(
    Request.Create().WithUrl("/some/thing").UsingGet()
  )
  .RespondWith(
    Response.Create()
      .WithBody(new byte[] { 48, 65, 6c, 6c, 6f })
  );
```

### Request Matching
WireMock supports matching of requests to stubs and verification queries using the following attributes:

* URL
* HTTP Method
* Query parameters
* Headers
* Cookies
* Request body


#### JSON Path
Deems a match if the attribute value is valid JSON and matches the JSON Path expression supplied.
A JSON body will be considered to match a path expression if the expression returns either a non-null single value (string, integer etc.), or a non-empty object or array.

```csharp
var server = FluentMockServer.Start();
server
  .Given(
    Request.Create().WithUrl("/some/thing").UsingGet()
      .WithBody(new JsonPathMatcher("$.things[?(@.name == 'RequiredThing')]"));
  )
  .RespondWith(Response.Create().WithBody("Hello"));
```

```
// matching
{ "things": { "name": "RequiredThing" } }
{ "things": [ { "name": "RequiredThing" }, { "name": "Wiremock" } ] }
// not matching
{ "price": 15 }
{ "things": { "name": "Wiremock" } }
```

#### XPath
Deems a match if the attribute value is valid XML and matches the XPath expression supplied.
An XML document will be considered to match if any elements are returned by the XPath evaluation.
WireMock delegates to [XPath2.Net](https://github.com/StefH/XPath2.Net), therefore it support up to XPath version 2.0.

```csharp
var server = FluentMockServer.Start();
server
  .Given(
    Request.Create().WithUrl("/some/thing").UsingGet()
	  .WithBody(new XPathMatcher("/todo-list[count(todo-item) = 3]"));
  )
  .RespondWith(Response.WithBody("Hello"));
```

Will match xml below:
```xml
<todo-list>
  <todo-item id='a1'>abc</todo-item>
  <todo-item id='a2'>def</todo-item>
  <todo-item id='a3'>xyz</todo-item>
</todo-list>
```

### Response Templating
Response headers and bodies can optionally be rendered using [Handlebars.Net](https://github.com/rexm/Handlebars.Net) templates.
This enables attributes of the request to be used in generating the response e.g. to pass the value of a request ID header as a response header or render an identifier from part of the URL in the response body. To use this functionality, add `.WithTransformer()` to the response builder.

Example:
```csharp
var server = FluentMockServer.Start();
server
  .Given(
    Request.Create().WithUrl("/some/thing").UsingGet()
  )
  .RespondWith(
    Response.Create()
      .WithStatusCode(200)
      .WithHeader("Content-Type", "text/plain")
      .WithBody("Hello world! Your path is {{request.path}.")
      .WithTransformer()
  );
```

#### The request model
The model of the request is supplied to the header and body templates. The following request attributes are available:

* `request.url` - URL path and query
* `request.path` - URL path
* `request.path.[<n>]` - URL path segment (zero indexed) e.g. request.path.[2]
* `request.query.<key>`- First value of a query parameter e.g. request.query.search
* `request.query.<key>.[<n>]`- nth value of a query parameter (zero indexed) e.g. request.query.search.[5]
* `request.headers.<key>` - First value of a request header e.g. request.headers.X-Request-Id
* `request.headers.[<key>]` - Header with awkward characters e.g. request.headers.[$?blah]
* `request.headers.<key>.[<n>]` - nth value of a header (zero indexed) e.g. request.headers.ManyThings.[1]
* `request.cookies.<key>` - Value of a request cookie e.g. request.cookies.JSESSIONID
* `request.body` - Request body text (avoid for non-text bodies)

##### Handlebars helpers
All of the standard helpers (template functions) provided by the C# Handlebars implementation plus all of the string helpers are available e.g.
`{{capitalize request.query.search}}`

### Stub priority
*TODO*

### Verify interactions
The server keeps a log of the received requests. You can use this log to verify the interactions that have been done with the server during a test.  
To get all the request received by the server, you just need to read property *RequestLogs*:
```csharp
var allRequests = server.RequestLogs;
```
If you need to be more specific on the requests that have been send to the server, you can use the very same fluent API that allows to define routes:
```csharp
var customerReadRequests = server.SearchLogsFor(
    Request.Create().WithUrl("/api/customer*").UsingGet()
); 
```

### Simulating delays
A server can be configured with a global delay that will be applied to all requests. To do so you need to call method FluentMockServer.AddRequestProcessingDelay() as below:
```csharp
var server = FluentMockServer.Start();

// add a delay of 30 seconds for all requests
server.AddRequestProcessingDelay(TimeSpan.FromSeconds(30));
```

Delays can also be configured at route level:
```csharp
var server = FluentMockServer.Start();
server
  .Given(Request.Create().WithUrl("/slow"))
  .RespondWith(
    Responses.Create()
      .WithStatusCode(200)
      .WithBody(@"{ ""msg"": ""Hello I'm a little bit slow!"" }")
      .WithDelay(TimeSpan.FromSeconds(10)
    )
  );
```

### Reset
The WireMock server can be reset at any time, removing all stub mappings and deleting the request log. If you’re using either of the UnitTest rules this will happen automatically at the start of every test case. However you can do it yourself via a call to `server.Reset()`.

### Getting all currently registered stub mappings
All stub mappings can be fetched in C# by calling `server.ListAllStubMappings()`.

## WireMock with your favourite test framework
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
    .Given(Request.Create().WithUrl("/foo").UsingGet())
    .RespondWith(
      Response.Create()
        .WithStatusCode(200)
        .WithBody(@"{ ""msg"": ""Hello world!"" }")
    );

  // when
  var response = _sut.DoSomething();
    
  // then
  Check.That(response).IsEqualTo(EXPECTED_RESULT);
    
  // and optionnaly
  Check.That(_server.SearchLogsFor(Request.WithUrl("/error*")).IsEmpty();
}

...
[TearDown]
public void ShutdownServer()
{
    _server.Stop();
}
```

## WireMock as a standalone process
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
        .Given(Request.Create().WithUrl(u => u.Contains("x")).UsingGet())
        .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""/x with FUNC 200""}"));

    server
        .Given(Request.Create().WithUrl("/*").UsingGet())
        .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""msg"": ""Hello world!""}")
        );

    server
        .Given(Request.Create().WithUrl("/data").UsingPost().WithBody(b => b.Contains("e")))
        .RespondWith(Response.Create()
            .WithStatusCode(201)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""data posted with FUNC 201""}"));

    server
        .Given(Request.Create().WithUrl("/data").UsingPost())
        .RespondWith(Response.Create()
            .WithStatusCode(201)
            .WithHeader("Content-Type", "application/json")
            .WithBody(@"{ ""result"": ""data posted with 201""}"));

    server
        .Given(Request.Create().WithUrl("/data").UsingDelete())
        .RespondWith(Response.Create()
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

### SSL
You can start a standalone mock server listening for HTTPS requests. To do so, there is just a flag to set when creating the server:
```csharp
var server = FluentMockServer.Start(port: 8443, ssl: true);
```
Obviously you need a certificate registered on your box, properly associated with your application and the port number that will be used. This is not really specific to WireMock, not very straightforward and hence the following stackoverflow thread might come handy: [Httplistener with https support](http://stackoverflow.com/questions/11403333/httplistener-with-https-support)

## Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface. The following interfaces are supported:

### /__admin/mappings
The mappings defined in the mock service.
* `GET    /__admin/mappings` --> Gets all defined mappings.
* `DELETE /__admin/mappings` --> TODO
* `POST   /__admin/mappings` --> TODO
* `DELETE /__admin/mappings` --> TODO

### /__admin/requests
Logged requests and responses received by the mock service.
* `GET /__admin/requests` --> Get received requests
* `GET /__admin/requests/{requestId}` --> TODO
* `POST /__admin/requests/reset` --> TODO
* `POST /__admin/requests/count` --> TODO
* `POST /__admin/requests/find` --> TODO
* `GET /__admin/requests/unmatched` --> TODO
* `GET /__admin/requests/unmatched/near-misses` --> TODO

## Simulating faults
Currently not done - need to get rid of HttpListener and use lower level TcpListener in order to be able to implement this properly
