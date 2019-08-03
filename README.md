# WireMock.Net
A C# .NET version based on [mock4net](https://github.com/alexvictoor/mock4net) which mimics the functionality from the JAVA based [WireMock.org](http://WireMock.org).

## Key Features
* HTTP response stubbing, matchable on URL/Path, headers, cookies and body content patterns
* Library can be used in unit tests and integration tests
* Runs as a standalone process, as windows service, as Azure/IIS or as docker
* Configurable via a fluent DotNet API, JSON files and JSON over HTTP
* Record/playback of stubs (proxying)
* Per-request conditional proxying
* Stateful behaviour simulation
* Response templating / transformation using Handlebars and extensions
* Can be used locally or in CI/CD scenarios.

## Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Gitter](https://img.shields.io/gitter/room/wiremock_dotnet/Lobby.svg)](https://gitter.im/wiremock_dotnet/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/WireMock-Net/WireMock.Net.svg)](https://github.com/WireMock-Net/WireMock.Net/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Build Azure** | [![Build Status Azure](https://stef.visualstudio.com/WireMock.Net/_apis/build/status/WireMock.Net)](https://stef.visualstudio.com/WireMock.Net/_build/latest?definitionId=7) |
| &nbsp;&nbsp;**CodeFactor** | [![CodeFactor](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net/badge)](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net)
| &nbsp;&nbsp;**Sonar Quality Gate** | [![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=alert_status)](https://sonarcloud.io/project/issues?id=wiremock) |
| &nbsp;&nbsp;**Sonar Bugs** | [![Sonar Bugs](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=bugs)](https://sonarcloud.io/project/issues?id=wiremock&resolved=false&types=BUG) |
| &nbsp;&nbsp;**Sonar Code Smells** | [![Sonar Code Smells](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=code_smells)](https://sonarcloud.io/project/issues?id=wiremock&resolved=false&types=CODE_SMELL) |
| &nbsp;&nbsp;**Sonar Coverage** | [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=coverage)](https://sonarcloud.io/component_measures?id=wiremock&metric=coverage) |
| &nbsp;&nbsp;**Codecov** | [![codecov](https://codecov.io/gh/WireMock-Net/WireMock.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/WireMock-Net/WireMock.Net) |
| &nbsp;&nbsp;**Coveralls** | [![Coverage Status](https://coveralls.io/repos/github/WireMock-Net/WireMock.Net/badge.svg?branch=master)](https://coveralls.io/github/WireMock-Net/WireMock.Net?branch=master) |

### NuGet packages

| | Official | Preview |
| - | - | - |
| &nbsp;&nbsp;**WireMock.Net** | [![NuGet Badge WireMock.Net](https://buildstats.info/nuget/WireMock.Net)](https://www.nuget.org/packages/WireMock.Net) |  [![MyGet Badge WireMock.Net](https://buildstats.info/myget/wiremock-net/WireMock.Net)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net)
| &nbsp;&nbsp;**WireMock.Net.StandAlone** | [![NuGet Badge WireMock.Net.StandAlone](https://buildstats.info/nuget/WireMock.Net.StandAlone)](https://www.nuget.org/packages/WireMock.Net.StandAlone) | [![MyGet Badge WireMock.Net.StandAlone](https://buildstats.info/myget/wiremock-net/WireMock.Net.StandAlone)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.StandAlone)
| &nbsp;&nbsp;**WireMock.Net.RestClient** | [![NuGet Badge WireMock.Net.RestClient](https://buildstats.info/nuget/WireMock.Net.RestClient)](https://www.nuget.org/packages/WireMock.Net.RestClient) | [![MyGet Badge WireMock.Net.RestClient](https://buildstats.info/myget/wiremock-net/WireMock.Net.RestClient)](https://www.myget.org/feed/wiremock-net/package/nuget/WireMock.Net.RestClient)


## Development
For the supported frameworks and build information, see [this](https://github.com/WireMock-Net/WireMock.Net/wiki/Development-Information) page.

## Stubbing
A core feature of WireMock.Net is the ability to return predefined HTTP responses for requests matching criteria.
See [Wiki : Stubbing](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing).

## Request Matching
WireMock.Net support advanced request-matching logic, see [Wiki : Request Matching](https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching).

## Response Templating
The response which is returned WireMock.Net can be changed using templating. This is described here [Wiki : Response Templating](https://github.com/WireMock-Net/WireMock.Net/wiki/Response-Templating).

## Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

## Using
WireMock.Net can be used in several ways:

### UnitTesting
You can use your favorite test framework and use WireMock within your tests, see
[Wiki : UnitTesting](https://github.com/StefH/WireMock.Net/wiki/Using-WireMock-in-UnitTests).

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
