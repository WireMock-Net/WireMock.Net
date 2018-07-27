# WireMock.Net
A C# .NET version based on [mock4net](https://github.com/alexvictoor/mock4net) which mimics the functionality from the JAVA based [WireMock.org](http://WireMock.org).

## Key Features
* HTTP response stubbing, matchable on URL/Path, headers, cookies and body content patterns
* Runs in unit tests, as a standalone process, as windows service, as Azure or IIS or as docker
* Configurable via a fluent DotNet API, JSON files and JSON over HTTP
* Record/playback of stubs
* Per-request conditional proxying
* Stateful behaviour simulation
* Configurable response delays

## Info
| | |
| --- | --- |
| ***Project*** | &nbsp; |
| &nbsp;&nbsp;**Chat** | [![Gitter](https://img.shields.io/gitter/room/wiremock_dotnet/Lobby.svg)](https://gitter.im/wiremock_dotnet/Lobby) |
| &nbsp;&nbsp;**Issues** | [![GitHub issues](https://img.shields.io/github/issues/WireMock-Net/WireMock.Net.svg)](https://github.com/WireMock-Net/WireMock.Net/issues) |
| | |
| ***Quality*** | &nbsp; |
| &nbsp;&nbsp;**Build** | [![Build status](https://ci.appveyor.com/api/projects/status/b3n6q3ygbww4lyls?svg=true)](https://ci.appveyor.com/project/StefH/wiremock-net) |
| &nbsp;&nbsp;**CodeFactor** | [![CodeFactor](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net/badge)](https://www.codefactor.io/repository/github/wiremock-net/wiremock.net)
| &nbsp;&nbsp;**Sonar Quality Gate** | [![Sonar Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=alert_status)](https://sonarcloud.io/project/issues?id=wiremock) |
| &nbsp;&nbsp;**Sonar Bugs** | [![Sonar Bugs](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=bugs)](https://sonarcloud.io/project/issues?id=wiremock&resolved=false&types=BUG) |
| &nbsp;&nbsp;**Sonar Code Smells** | [![Sonar Code Smells](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=code_smells)](https://sonarcloud.io/project/issues?id=wiremock&resolved=false&types=CODE_SMELL) |
| &nbsp;&nbsp;**Sonar Coverage** | [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=wiremock&metric=coverage)](https://sonarcloud.io/component_measures?id=wiremock&metric=coverage) |
| &nbsp;&nbsp;**Coveralls** | [![Coverage Status](https://coveralls.io/repos/github/WireMock-Net/WireMock.Net/badge.svg?branch=master)](https://coveralls.io/github/WireMock-Net/WireMock.Net?branch=master) |
| |
| ***Nuget*** | &nbsp; |
| &nbsp;&nbsp;**WireMock.Net** | [![NuGet Badge WireMock.Net](https://buildstats.info/nuget/WireMock.Net)](https://www.nuget.org/packages/WireMock.Net) |
| &nbsp;&nbsp;**WireMock.Net.StandAlone** | [![NuGet Badge WireMock.Net.StandAlone](https://buildstats.info/nuget/WireMock.Net.StandAlone)](https://www.nuget.org/packages/WireMock.Net.StandAlone) |

### Frameworks
The following frameworks are supported:
- net 4.5.2 and up & net 4.6 and up
- netstandard 1.3 & netstandard 2.0

### Build info
To build you need:
- Microsoft .NET Framework 4.5.2 Developer Pack (https://www.microsoft.com/en-us/download/details.aspx?id=42637)
- Microsoft .NET Framework 4.6 Targeting Pack (https://www.microsoft.com/en-us/download/confirmation.aspx?id=48136)
- Microsoft .NET Framework 4.6.2 Developer Pack (https://www.microsoft.com/en-us/download/confirmation.aspx?id=53321)
- .NET Core 2.0 (https://www.microsoft.com/net/core)

## Stubbing
A core feature of WireMock.Net is the ability to return canned/predefined HTTP responses for requests matching criteria, see [Wiki : Stubbing & Request Matching](https://github.com/WireMock-Net/WireMock.Net/wiki/Stubbing-and-Request-Matching).

## Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface, see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

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
