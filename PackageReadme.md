## WireMock.Net
Lightweight Http Mocking Server for .NET, inspired by WireMock.org (from the Java landscape).

### :star: Key Features
* HTTP response stubbing, matchable on URL/Path, headers, cookies and body content patterns
* Library can be used in unit tests and integration tests
* Runs as a standalone process, as windows service, as Azure/IIS or as docker
* Configurable via a fluent C# .NET API, JSON files and JSON over HTTP
* Record/playback of stubs (proxying)
* Per-request conditional proxying
* Stateful behaviour simulation
* Response templating / transformation using Handlebars and extensions
* Can be used locally or in CI/CD scenarios
* Can be used for Aspire Distributed Application testing 

### :star: Stubbing
A core feature of WireMock.Net is the ability to return predefined HTTP responses for requests matching criteria.
See [Wiki : Stubbing](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing).

### :star: Request Matching
WireMock.Net support advanced request-matching logic, see [Wiki : Request Matching](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching).

### :star: Response Templating
The response which is returned WireMock.Net can be changed using templating. This is described here [Wiki : Response Templating](https://github.com/WireMock-Net/WireMock.Net/wiki/Response-Templating).

### :star: Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

### :star: Using
WireMock.Net can be used in several ways:

#### UnitTesting
You can use your favorite test framework and use WireMock within your tests, see
[Wiki : UnitTesting](https://github.com/StefH/WireMock.Net/wiki/Using-WireMock-in-UnitTests).

### Unit/Integration Testing using Testcontainers.DotNet
See [Wiki : WireMock.Net.Testcontainers](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock.Net.Testcontainers) on how to build a WireMock.Net Docker container which can be used in Unit/Integration testing.

### Unit/Integration Testing using an an Aspire Distributed Application
See [Wiki : WireMock.Net.Aspire](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock.Net.Aspire) on how to use WireMock.Net as an Aspire Hosted application to do Unit/Integration testing.

#### As a dotnet tool
It's simple to install WireMock.Net as (global) dotnet tool, see [Wiki : dotnet tool](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-dotnet-tool).

#### As standalone process / console application
This is quite straight forward to launch a mock server within a console application, see [Wiki : Standalone Process](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-a-standalone-process).

#### As a Windows Service
You can also run WireMock.Net as a Windows Service, follow this [WireMock-as-a-Windows-Service](https://github.com/WireMock-Net/WireMock.Net/wiki/WireMock-as-a-Windows-Service).

#### As a Web Job in Azure or application in IIS
See this link [WireMock-as-a-(Azure)-Web-App](https://github.com/WireMock-Net/WireMock.Net/wiki/WireMock-as-a-(Azure)-Web-App)

#### In a docker container
There is also a Linux and Windows-Nano container available at [hub.docker.com](https://hub.docker.com/r/sheyenrath).
For more details see also [Docker](https://github.com/WireMock-Net/WireMock.Net-docker).

#### HTTPS / SSL
More details on using HTTPS (SSL) can be found here [Wiki : HTTPS](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-HTTPS-(SSL))

## :books: Documentation
For more info, see also this WIKI page: [What is WireMock.Net](https://github.com/WireMock-Net/WireMock.Net/wiki/What-Is-WireMock.Net).
