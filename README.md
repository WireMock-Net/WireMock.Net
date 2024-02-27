# WireMock.Net
A C# .NET version based on [mock4net](https://github.com/alexvictoor/mock4net) which mimics the functionality from the JAVA based [WireMock](http://WireMock.org).

For more info, see also this WIKI page: [What is WireMock.Net](https://github.com/WireMock-Net/WireMock.Net/wiki/What-Is-WireMock.Net).

## :star: Key Features
* HTTP response stubbing, matchable on URL/Path, headers, cookies and body content patterns
* Library can be used in unit tests and integration tests
* Runs as a standalone process, as windows service, as Azure/IIS or as docker
* Configurable via a fluent C# .NET API, JSON files and JSON over HTTP
* Record/playback of stubs (proxying)
* Per-request conditional proxying
* Stateful behaviour simulation
* Response templating / transformation using Handlebars and extensions
* Can be used locally or in CI/CD scenarios

## :memo: Blogs
- [mstack.nl : Generate C# Code from Mapping(s)](https://mstack.nl/blog/20230201-wiremock.net-tocode/)
- [mstack.nl : Chaos Engineering with Fault Injections](https://mstack.nl/blogs/wiremock-net-chaos-engineering-with-fault-injections/)
- [mstack.nl : gRPC / ProtoBuf Support](https://mstack.nl/blogs/wiremock-net-grpc/)


## :computer: Project Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Slack](https://badgen.net/badge/icon/slack?icon=slack&label)](https://slack.wiremock.org/) [![Gitter](https://img.shields.io/gitter/room/wiremock_dotnet/Lobby.svg)](https://gitter.im/wiremock_dotnet/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/WireMock-Net/WireMock.Net.svg)](https://github.com/WireMock-Net/WireMock.Net/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Build Azure** | [![Build Status Azure](https://stef.visualstudio.com/WireMock.Net/_apis/build/status/WireMock.Net)](https://stef.visualstudio.com/WireMock.Net/_build/latest?definitionId=7) |
| &nbsp;&nbsp;**Quality** | [![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=alert_status)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net) [![CodeFactor](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net/badge)](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net) |
| &nbsp;&nbsp;**Sonar Bugs** | [![Sonar Bugs](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=bugs)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net&resolved=false&types=BUG) [![Sonar Code Smells](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=code_smells)](https://sonarcloud.io/project/issues?id=WireMock-Net_WireMock.Net&resolved=false&types=CODE_SMELL) |
| &nbsp;&nbsp;**Coverage** | [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=WireMock-Net_WireMock.Net&metric=coverage)](https://sonarcloud.io/component_measures?id=WireMock-Net_WireMock.Net&metric=coverage) [![codecov](https://codecov.io/gh/WireMock-Net/WireMock.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/WireMock-Net/WireMock.Net)|

### :package: NuGet packages

| | Info | Official | Preview [:information_source:](https://github.com/WireMock-Net/WireMock.Net/wiki/MyGet-preview-versions) |
| - | - | - |
| &nbsp;&nbsp;**WireMock.Net** | Main project | [![NuGet Badge WireMock.Net](https://buildstats.info/nuget/WireMock.Net)](https://www.nuget.org/packages/WireMock.Net) |  [![MyGet Badge WireMock.Net](https://buildstats.info/myget/wiremock-net/WireMock.Net?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net)
| &nbsp;&nbsp;**WireMock.Net.StandAlone** | StandAlone | [![NuGet Badge WireMock.Net](https://buildstats.info/nuget/WireMock.Net.StandAlone)](https://www.nuget.org/packages/WireMock.Net.StandAlone) | [![MyGet Badge WireMock.Net.StandAlone](https://buildstats.info/myget/wiremock-net/WireMock.Net.StandAlone?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.StandAlone)
| &nbsp;&nbsp;**WireMock.Net.GraphQL** | [GraphQL Support](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching-GraphQLMatcher) | [![NuGet Badge WireMock.Net.GraphQL](https://buildstats.info/nuget/WireMock.Net.GraphQL)](https://www.nuget.org/packages/WireMock.Net.GraphQL) | [![MyGet Badge WireMock.Net.GraphQL](https://buildstats.info/myget/wiremock-net/WireMock.Net.GraphQL?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.GraphQL)
| &nbsp;&nbsp;**WireMock.Net.MimeKitLite** | [MimePart Support](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching-MimePartMatcher) | [![NuGet Badge WireMock.Net.MimeKitLite](https://buildstats.info/nuget/WireMock.Net.MimeKitLite)](https://www.nuget.org/packages/WireMock.Net.MimeKitLite) | [![MyGet Badge WireMock.Net.MimeKitLite](https://buildstats.info/myget/wiremock-net/WireMock.Net.MimeKitLite?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.MimeKitLite)
| &nbsp;&nbsp;**WireMock.Net.ProtoBuf** | [ProtoBuf gRPC Support](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching-ProtoBuf) | [![NuGet Badge WireMock.Net.ProtoBuf](https://buildstats.info/nuget/WireMock.Net.ProtoBuf)](https://www.nuget.org/packages/WireMock.Net.ProtoBuf) | [![MyGet Badge WireMock.Net.ProtoBuf](https://buildstats.info/myget/wiremock-net/WireMock.Net.ProtoBuf?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.ProtoBuf)
| &nbsp;&nbsp;**WireMock.Net.Matchers.CSharpCode** | [C# Code Matcher](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching-CSharpCode) | [![NuGet Badge WireMock.Net.Matchers.CSharpCode](https://buildstats.info/nuget/WireMock.Net.Matchers.CSharpCode)](https://www.nuget.org/packages/WireMock.Net.Matchers.CSharpCode) | [![MyGet Badge WireMock.Net.Matchers.CSharpCode](https://buildstats.info/myget/wiremock-net/WireMock.Net.Matchers.CSharpCode?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Matchers.CSharpCode)
| &nbsp;&nbsp;**WireMock.Net.OpenApiParser** | OpenApiParser | [![NuGet Badge WireMock.Net.OpenApiParser](https://buildstats.info/nuget/WireMock.Net.OpenApiParser)](https://www.nuget.org/packages/WireMock.Net.OpenApiParser) | [![MyGet Badge WireMock.Net.OpenApiParser](https://buildstats.info/myget/wiremock-net/WireMock.Net.OpenApiParser?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.OpenApiParser)
| &nbsp;&nbsp;**WireMock.Net.FluentAssertions** | FluentAssertions Extensions | [![NuGet Badge WireMock.Net.FluentAssertions](https://buildstats.info/nuget/WireMock.Net.FluentAssertions)](https://www.nuget.org/packages/WireMock.Net.FluentAssertions) | [![MyGet Badge WireMock.Net.FluentAssertions](https://buildstats.info/myget/wiremock-net/WireMock.Net.FluentAssertions?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.FluentAssertions)
| &nbsp;&nbsp;**WireMock.Net.xUnit** | xUnit logging support | [![NuGet Badge WireMock.Net.xUnit](https://buildstats.info/nuget/WireMock.Net.xUnit)](https://www.nuget.org/packages/WireMock.Net.xUnit) | [![MyGet Badge WireMock.Net.xUnit](https://buildstats.info/myget/wiremock-net/WireMock.Net.xUnit?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.xUnit)
| &nbsp;&nbsp;**WireMock.Net.Testcontainers** | [Testcontainers Support](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock.Net.Testcontainers) | [![NuGet Badge WireMock.Net.Testcontainers](https://buildstats.info/nuget/WireMock.Net.Testcontainers)](https://www.nuget.org/packages/WireMock.Net.Testcontainers) | [![MyGet Badge WireMock.Net.Testcontainers](https://buildstats.info/myget/wiremock-net/WireMock.Net.Testcontainers?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.Testcontainers)
| &nbsp;&nbsp;**WireMock.Net.RestClient** | RestClient for WireMock.Net| [![NuGet Badge WireMock.Net.RestClient](https://buildstats.info/nuget/WireMock.Net.RestClient)](https://www.nuget.org/packages/WireMock.Net.RestClient) | [![MyGet Badge WireMock.Net.RestClient](https://buildstats.info/myget/wiremock-net/WireMock.Net.RestClient?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.RestClient)
| &nbsp;&nbsp;**WireMock.Org.RestClient** | RestClient for WireMock.Org| [![NuGet Badge WireMock.Org.RestClient](https://buildstats.info/nuget/WireMock.Org.RestClient)](https://www.nuget.org/packages/WireMock.Org.RestClient) | [![MyGet Badge WireMock.Org.RestClient](https://buildstats.info/myget/wiremock-net/WireMock.Org.RestClient?includePreReleases=true)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Org.RestClient)


## :memo: Development
For the supported frameworks and build information, see [this](https://github.com/WireMock-Net/WireMock.Net/wiki/Development-Information) page.

## :star: Stubbing
A core feature of WireMock.Net is the ability to return predefined HTTP responses for requests matching criteria.
See [Wiki : Stubbing](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing).

## :star: Request Matching
WireMock.Net support advanced request-matching logic, see [Wiki : Request Matching](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching).

## :star: Response Templating
The response which is returned WireMock.Net can be changed using templating. This is described here [Wiki : Response Templating](https://github.com/WireMock-Net/WireMock.Net/wiki/Response-Templating).

## :star: Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

## :star: Using
WireMock.Net can be used in several ways:

### UnitTesting
You can use your favorite test framework and use WireMock within your tests, see
[Wiki : UnitTesting](https://github.com/StefH/WireMock.Net/wiki/Using-WireMock-in-UnitTests).

### Unit/Integration Testing using Testcontainers.DotNet
You can use [Wiki : WireMock.Net.Testcontainers](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock.Net.Testcontainers) to build a WireMock.Net Docker container which can be used in Unit/Integration testing.

### As a dotnet tool
It's simple to install WireMock.Net as (global) dotnet tool, see [Wiki : dotnet tool](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-dotnet-tool).

### As standalone process / console application
This is quite straight forward to launch a mock server within a console application, see [Wiki : Standalone Process](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-a-standalone-process).

### As a Windows Service
You can also run WireMock.Net as a Windows Service, follow this [WireMock-as-a-Windows-Service](https://github.com/WireMock-Net/WireMock.Net/wiki/WireMock-as-a-Windows-Service).

### As a Web Job in Azure or application in IIS
See this link [WireMock-as-a-(Azure)-Web-App](https://github.com/WireMock-Net/WireMock.Net/wiki/WireMock-as-a-(Azure)-Web-App)

### In a docker container
There is also a Linux and Windows-Nano container available at [hub.docker.com](https://hub.docker.com/r/sheyenrath).
For more details see also [Docker](https://github.com/WireMock-Net/WireMock.Net-docker).

#### HTTPS / SSL
More details on using HTTPS (SSL) can be found here [Wiki : HTTPS](https://github.com/WireMock-Net/WireMock.Net/wiki/Using-HTTPS-(SSL))
