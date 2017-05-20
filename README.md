# WireMock.Net
A C# .NET version based on [mock4net](https://github.com/alexvictoor/mock4net) which mimics the functionality from the JAVA based http://WireMock.org

[![Build status](https://ci.appveyor.com/api/projects/status/b3n6q3ygbww4lyls?svg=true)](https://ci.appveyor.com/project/StefH/wiremock-net)
[![codecov](https://codecov.io/gh/StefH/WireMock.Net/branch/master/graph/badge.svg)](https://codecov.io/gh/StefH/WireMock.Net)
[![Coverage Status](https://coveralls.io/repos/github/StefH/WireMock.Net/badge.svg?branch=master)](https://coveralls.io/github/StefH/WireMock.Net?branch=master)
[![NuGet Badge](https://buildstats.info/nuget/WireMock.Net)](https://www.nuget.org/packages/WireMock.Net)
[![GitHub issues](https://img.shields.io/github/issues/StefH/WireMock.Net.svg)](https://github.com/StefH/WireMock.Net/issues)
[![GitHub stars](https://img.shields.io/github/stars/StefH/WireMock.Net.svg)](https://github.com/StefH/WireMock.Net/stargazers)

### Frameworks
The following frameworks are supported:
- net 4.5
- net 4.5.2 and up
- netstandard 1.3

## Stubbing
A core feature of WireMock.Net is the ability to return canned/predefined HTTP responses for requests matching criteria, see [Wiki : Stubbing](https://github.com/StefH/WireMock.Net/wiki/Stubbing).

## Using WireMock in UnitTest framework
You can use your favorite test framework and use WireMock within your tests, see
[Wiki : UnitTesting](https://github.com/StefH/WireMock.Net/wiki/Using-WireMock-in-UnitTests).

## Admin API Reference
The WireMock admin API provides functionality to define the mappings via a http interface, see [Wiki : Admin API Reference](https://github.com/StefH/WireMock.Net/wiki/Admin-API-Reference).

## WireMock as a standalone process
This is quite straight forward to launch a mock server within a console application, see [Wiki : standalone](https://github.com/StefH/WireMock.Net/wiki/WireMock-as-a-standalone-process).

### SSL
You can start a standalone mock server listening for HTTPS requests. To do so, there is just a flag to set when creating the server:
```csharp
var server = FluentMockServer.Start(port: 8443, ssl: true);
```
Obviously you need a certificate registered on your box, properly associated with your application and the port number that will be used. This is not really specific to WireMock, not very straightforward and hence the following stackoverflow thread might come handy: [Httplistener with https support](http://stackoverflow.com/questions/11403333/httplistener-with-https-support)
