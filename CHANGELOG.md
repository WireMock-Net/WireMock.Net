# 1.7.5-preview-02 (03 April 2025)
- [#1265](https://github.com/WireMock-Net/WireMock.Net/pull/1265) - Fix construction of path in OpenApiParser [bug] contributed by [StefH](https://github.com/StefH)
- [#1269](https://github.com/WireMock-Net/WireMock.Net/pull/1269) - Server-Sent Events [feature] contributed by [StefH](https://github.com/StefH)
- [#1264](https://github.com/WireMock-Net/WireMock.Net/issues/1264) - OpenApiParser - Construction of path possibly incorrect [bug]

# 1.7.4 (27 February 2025)
- [#1254](https://github.com/WireMock-Net/WireMock.Net/issues/1254) - FindLogEntries exception 'Destination array was not long enough' [bug]

# 1.7.3 (24 February 2025)
- [#1247](https://github.com/WireMock-Net/WireMock.Net/issues/1247) - API call isn't matched when using an empty query string parameter [bug]

# 1.7.2 (12 February 2025)
- [#1239](https://github.com/WireMock-Net/WireMock.Net/issues/1239) - How to use WiremockContainerBuilder for grpc using http2 [feature]
- [#1249](https://github.com/WireMock-Net/WireMock.Net/issues/1249) - Add protodefinition and refer it from mapping [feature]

# 1.7.1 (26 January 2025)
- [#1233](https://github.com/WireMock-Net/WireMock.Net/issues/1233) - GRPC mappings are not created correctly when created through Admin API [bug]

# 1.6.12 (21 January 2025)
- [#1227](https://github.com/WireMock-Net/WireMock.Net/issues/1227) - unable to call grpc method with namespace [bug]
- [#1228](https://github.com/WireMock-Net/WireMock.Net/issues/1228) - how to set datetime for grpc field [bug]
- [#1234](https://github.com/WireMock-Net/WireMock.Net/issues/1234) - FindLogEntries regression [bug]
- [#1240](https://github.com/WireMock-Net/WireMock.Net/issues/1240) - Method 'get_Category' in type 'WireMock.Transformers.Handlebars.FileHelpers' [bug]

# 1.6.11 (02 January 2025)
- [#1092](https://github.com/WireMock-Net/WireMock.Net/issues/1092) - FindLogEntries present in WireMockServer but not IWireMockServer [feature]
- [#1192](https://github.com/WireMock-Net/WireMock.Net/issues/1192) - Feature: add URL assertion excluding query parameters [feature]
- [#1204](https://github.com/WireMock-Net/WireMock.Net/issues/1204) - Supplying Values From Request for Webhook Body and Headers [feature]
- [#1217](https://github.com/WireMock-Net/WireMock.Net/issues/1217) - Order of WireMockContainerBuilder WithX calls caused duplicate Networks in Configuration [bug]

# 1.6.10 (15 December 2024)
- [#1188](https://github.com/WireMock-Net/WireMock.Net/issues/1188) - WithWatchStaticMappings doesn't respect new files [bug]

# 1.6.9 (06 December 2024)
- [#1210](https://github.com/WireMock-Net/WireMock.Net/issues/1210) - JsonPartialMatcher fails to match on property name that JsonMatcher matches [bug]

# 1.6.8 (24 November 2024)
- [#1201](https://github.com/WireMock-Net/WireMock.Net/issues/1201) - Mapping file parse errors are not logged to the console [feature]
- [#1209](https://github.com/WireMock-Net/WireMock.Net/issues/1209) - Upgrade of GraphQL libs to the latest [feature]
- [#1212](https://github.com/WireMock-Net/WireMock.Net/issues/1212) - Response Body Does Not Include Text After Path Segment [bug]

# 1.6.7 (17 October 2024)
- [#1144](https://github.com/WireMock-Net/WireMock.Net/issues/1144) - Using google.protobuf.Empty as response results in a bad gRPC response [bug]
- [#1153](https://github.com/WireMock-Net/WireMock.Net/issues/1153) - Grpc support for multiple proto files [feature]
- [#1193](https://github.com/WireMock-Net/WireMock.Net/issues/1193) - Snyk issue : Regular Expression Denial of Service  [bug]

# 1.6.6 (01 October 2024)
- [#1184](https://github.com/WireMock-Net/WireMock.Net/issues/1184) - .WithBodyFromFile() + .WithTransformer(transformContentFromBodyAsFile: true) = empty string [bug]
- [#1186](https://github.com/WireMock-Net/WireMock.Net/issues/1186) - WithMappings path is null on Build() call [bug]

# 1.6.4 (25 September 2024)
- [#1146](https://github.com/WireMock-Net/WireMock.Net/issues/1146) - Bump Request CS-Script 4.8.13 to 4.8.17 [feature]
- [#1167](https://github.com/WireMock-Net/WireMock.Net/issues/1167) - Admin API fails to create a mapping with Request Header matching using WildCardMatcher [bug]
- [#1168](https://github.com/WireMock-Net/WireMock.Net/issues/1168) - Numbers in text/plain content is parsed as JSON. [bug]
- [#1176](https://github.com/WireMock-Net/WireMock.Net/issues/1176) - WireMock.NET TestContainer Dependency Constraint Issue [bug]

# 1.6.3 (07 September 2024)
- [#1154](https://github.com/WireMock-Net/WireMock.Net/issues/1154) - Listen on all ips [bug]

# 1.6.2 (04 September 2024)
- [#1151](https://github.com/WireMock-Net/WireMock.Net/issues/1151) - MappingsToCSharpCode should use RegexMatcher when specified [bug]
- [#1164](https://github.com/WireMock-Net/WireMock.Net/issues/1164) - WithParam not working. [bug]

# 1.6.1 (22 August 2024)
- [#1159](https://github.com/WireMock-Net/WireMock.Net/issues/1159) - RegexMatchTimeoutException when trying to parse HTTP version [bug]

# 1.6.0 (16 August 2024)
- [#720](https://github.com/WireMock-Net/WireMock.Net/issues/720) - Response Header Content-Length not available when call HEAD Method [feature]
- [#1145](https://github.com/WireMock-Net/WireMock.Net/issues/1145) - Response is auto converting string to guid [bug]
- [#1156](https://github.com/WireMock-Net/WireMock.Net/issues/1156) - FormUrlEncodedMatcher is not requiring to match all properties when MatchOperator.And  [bug]

# 1.5.62 (27 July 2024)
- [#1143](https://github.com/WireMock-Net/WireMock.Net/issues/1143) - FormEncoded Request fails (404 Not Found) if key value pairs order in mapping is different from request body order [bug]

# 1.5.61 (22 July 2024)
- [#1118](https://github.com/WireMock-Net/WireMock.Net/issues/1118) - Generating mappings from Payroc open-api file gives ArgumentException: Property with the same name already exists on object [bug]
- [#1139](https://github.com/WireMock-Net/WireMock.Net/issues/1139) - Allow WithMappings to support scanning SubDirectories when building a WireMockContainer [feature]
- [#1140](https://github.com/WireMock-Net/WireMock.Net/issues/1140) - WireMock.Org nullable properties and defaults [bug]

# 1.5.60 (09 July 2024)
- [#1119](https://github.com/WireMock-Net/WireMock.Net/issues/1119) - Error in RequestMessageMultiPartMatcher [bug]
- [#1121](https://github.com/WireMock-Net/WireMock.Net/issues/1121) - XML transformation [feature]

# 1.5.58 (08 June 2024)
- [#1117](https://github.com/WireMock-Net/WireMock.Net/issues/1117) - AbstractJsonPartialMatcher: Regex Value is Uppercased when IgnoreCase is set to true [bug]

# 1.5.56 (03 June 2024)
- [#1110](https://github.com/WireMock-Net/WireMock.Net/issues/1110) - Connection prematurely closed BEFORE response  [bug]

# 1.5.53 (08 May 2024)
- [#1095](https://github.com/WireMock-Net/WireMock.Net/issues/1095) - When using C# code generation WithBody() matcher is not generated for POST Request [bug]

# 1.5.52 (06 April 2024)
- [#1088](https://github.com/WireMock-Net/WireMock.Net/issues/1088) - Regex support for JsonMatcher  [feature]

# 1.5.51 (20 March 2024)
- [#1084](https://github.com/WireMock-Net/WireMock.Net/issues/1084) - FluentAssertions - Actual body is not displayed in error message when using Json Body [bug]

# 1.5.50 (12 March 2024)
- [#1074](https://github.com/WireMock-Net/WireMock.Net/issues/1074) - FluentAssertions extensions do not filter headers correctly [bug]
- [#1075](https://github.com/WireMock-Net/WireMock.Net/issues/1075) - FluentAssertions extensions are not open for extension [feature]

# 1.5.49 (06 March 2024)
- [#1077](https://github.com/WireMock-Net/WireMock.Net/issues/1077) - ProtoBufMatcher not working when proto package name contains dots [bug]

# 1.5.47 (25 January 2024)
- [#1048](https://github.com/WireMock-Net/WireMock.Net/issues/1048) - WithoutHeader fluent assertion [feature]
- [#1054](https://github.com/WireMock-Net/WireMock.Net/issues/1054) - WireMock.Net 1.5.46 is incompatible with TestContainers 3.7.0 (issue 1) [bug]
- [#1059](https://github.com/WireMock-Net/WireMock.Net/issues/1059) - WireMock.Net 1.5.46 is incompatible with TestContainers 3.7.0 (issue 2) [bug]

# 1.5.45 (21 December 2023)
- [#1039](https://github.com/WireMock-Net/WireMock.Net/issues/1039) - [Admin API] Find a request that matched a given mapping [feature]

# 1.5.44 (14 December 2023)
- [#1033](https://github.com/WireMock-Net/WireMock.Net/issues/1033) - How to get a Random Long? [bug]
- [#1037](https://github.com/WireMock-Net/WireMock.Net/issues/1037) - Make mapping filenames more user friendly [feature]

# 1.5.43 (11 December 2023)
- [#592](https://github.com/WireMock-Net/WireMock.Net/issues/592) - Proxy all requests - even a repeated one [feature]
- [#1024](https://github.com/WireMock-Net/WireMock.Net/issues/1024) - Scenario with proxy not removing route prefix [feature]

# 1.5.42 (09 December 2023)
- [#1021](https://github.com/WireMock-Net/WireMock.Net/issues/1021) - GetAdminMappingsResult in WireMock.Org.Abstractions should contain list of mappings [bug]
- [#1030](https://github.com/WireMock-Net/WireMock.Net/issues/1030) - Reset resets only mappings and logentries, not scenarios. [bug]

# 1.5.41 (04 December 2023)
- [#984](https://github.com/WireMock-Net/WireMock.Net/issues/984) - GraphQL Schema validation with custom scalars [feature]

# 1.5.39 (09 October 2023)
- [#997](https://github.com/WireMock-Net/WireMock.Net/issues/997) - JmesPathMatcher or and MatchOperator working in version 1.5.34 but not 1.5.35 [bug]

# 1.5.37 (27 September 2023)
- [#1003](https://github.com/WireMock-Net/WireMock.Net/issues/1003) - Store Mapping per POST request ignores &quot;IgnoreCase&quot; property of HeaderModel [bug]

# 1.5.36 (21 September 2023)
- [#974](https://github.com/WireMock-Net/WireMock.Net/issues/974) - HttpClient extension methods causes ambiguous invocations in .NET 7 [bug]
- [#1001](https://github.com/WireMock-Net/WireMock.Net/issues/1001) - SaveUnmatchedRequests stopped working [bug]

# 1.5.34 (04 August 2023)
- [#988](https://github.com/WireMock-Net/WireMock.Net/issues/988) - v1.5.33  Returns always StatusCode 500 [bug]

# 1.5.33 (03 August 2023)
- [#968](https://github.com/WireMock-Net/WireMock.Net/issues/968) - Using request multipart in response template [feature]
- [#969](https://github.com/WireMock-Net/WireMock.Net/issues/969) - Multipart validation [feature]
- [#970](https://github.com/WireMock-Net/WireMock.Net/issues/970) - Loop through xml elements in handlebars template [feature]
- [#971](https://github.com/WireMock-Net/WireMock.Net/issues/971) - JsonPartialMatcher - match guid and string [feature]

# 1.5.32 (15 July 2023)
- [#965](https://github.com/WireMock-Net/WireMock.Net/issues/965) - JsonPathMatcher does not match json body nested objects [bug]
- [#967](https://github.com/WireMock-Net/WireMock.Net/issues/967) - &#11088;10 million downloads ! &#11088; [feature]

# 1.5.30 (28 June 2023)
- [#958](https://github.com/WireMock-Net/WireMock.Net/issues/958) - [FluentAssertions] Should().HaveReceivedACall().WithHeader() only checks the first header with the matching key. [bug]

# 1.5.29 (22 June 2023)
- [#953](https://github.com/WireMock-Net/WireMock.Net/issues/953) - How to use environment variable [feature]

# 1.5.27 (03 June 2023)
- [#928](https://github.com/WireMock-Net/WireMock.Net/issues/928) - TypeLoadException when using WithHeader method. [bug]
- [#945](https://github.com/WireMock-Net/WireMock.Net/issues/945) - Webhook logging [feature]

# 1.5.26 (25 May 2023)
- [#941](https://github.com/WireMock-Net/WireMock.Net/issues/941) - RequestMessage.GetParameter method missing from IRequestMessage interface [feature]

# 1.5.22 (08 April 2023)
- [#912](https://github.com/WireMock-Net/WireMock.Net/issues/912) - Feature: adding excluded params to proxy and records settings [feature]

# 1.5.21 (22 March 2023)
- [#901](https://github.com/WireMock-Net/WireMock.Net/issues/901) - Matching one form-urlencoded value [feature]

# 1.5.20 (19 March 2023)
- [#906](https://github.com/WireMock-Net/WireMock.Net/issues/906) - Upgrade to 1.5.19 breaks a form data test [bug]

# 1.5.18 (09 March 2023)
- [#897](https://github.com/WireMock-Net/WireMock.Net/issues/897) - WebHostBuilder.ConfigureServices method not found when using nunit3testadapter 4.4.0 [bug]
- [#899](https://github.com/WireMock-Net/WireMock.Net/issues/899) - Ignore OPTIONS request when using proxyandrecord [feature]

# 1.5.16 (01 February 2023)
- [#879](https://github.com/WireMock-Net/WireMock.Net/issues/879) - Possibility to pass a X509Certificate2 to WithProxy() or specifiy certificate loading options [feature]

# 1.5.14 (24 January 2023)
- [#701](https://github.com/WireMock-Net/WireMock.Net/issues/701) - Allow to create MappingModel from c# to be able to configure local and remote mocks similarly [feature]
- [#867](https://github.com/WireMock-Net/WireMock.Net/issues/867) - Can I build mappings with code and save them to JSON-file without starting server [feature]
- [#870](https://github.com/WireMock-Net/WireMock.Net/issues/870) - Can not unsubscribe from LogEntriesChanged event. [bug]

# 1.5.13 (11 December 2022)
- [#856](https://github.com/WireMock-Net/WireMock.Net/issues/856) - Inconsistent result with overlapping (duplicate) request [bug]

# 1.5.11 (24 November 2022)
- [#846](https://github.com/WireMock-Net/WireMock.Net/issues/846) - Exception ArgumentOutOfRangeException [bug]

# 1.5.9 (29 October 2022)
- [#826](https://github.com/WireMock-Net/WireMock.Net/issues/826) - Dynamic Body not to be cached when a Func is used to created the body [feature]

# 1.5.8 (16 October 2022)
- [#814](https://github.com/WireMock-Net/WireMock.Net/issues/814) - WithHeader cannot handle multiple requests with the same header key values [bug]
- [#815](https://github.com/WireMock-Net/WireMock.Net/issues/815) - Why does UsingMethod check _callscount? [bug]
- [#822](https://github.com/WireMock-Net/WireMock.Net/issues/822) - Webhook with generic url, body and custom header values [feature]

# 1.5.7 (11 October 2022)
- [#819](https://github.com/WireMock-Net/WireMock.Net/issues/819) - Can I preserve Mapping title and matchers for proxy response? [feature]

# 1.5.6 (12 September 2022)
- [#801](https://github.com/WireMock-Net/WireMock.Net/issues/801) - Webhook Delays [feature]

# 1.5.5 (03 September 2022)
- [#772](https://github.com/WireMock-Net/WireMock.Net/issues/772) - How to get matched mapping by HttpRequest or HttpRequestMessage [feature]

# 1.5.4 (24 August 2022)
- [#775](https://github.com/WireMock-Net/WireMock.Net/issues/775) - When &quot;StartAdminInterface&quot; is true then each time is generated new mapping from the proxy [bug]
- [#784](https://github.com/WireMock-Net/WireMock.Net/issues/784) - Response body is missing in generated pact file when IBodyResponseBuilder.WithBody is used [bug]
- [#785](https://github.com/WireMock-Net/WireMock.Net/issues/785) - Support for PEM certificates when using ssl [feature]
- [#788](https://github.com/WireMock-Net/WireMock.Net/issues/788) - Request body is missing in generated pact file for requests that include matching on request body [bug]
- [#796](https://github.com/WireMock-Net/WireMock.Net/issues/796) - RequestMessageHeaderMatcher with MatchBehaviour.RejectOnMatch reverses match results twice [bug]

# 1.5.3 (29 July 2022)
- [#776](https://github.com/WireMock-Net/WireMock.Net/issues/776) - Update Scriban.Signed to support more functions, e.g math.random [feature]

# 1.5.1 (08 July 2022)
- [#764](https://github.com/WireMock-Net/WireMock.Net/issues/764) - Wrong mapping of method GetAdminMappingsAsync from IWireMockOrgApi [bug]

# 1.4.43 (21 May 2022)
- [#756](https://github.com/WireMock-Net/WireMock.Net/issues/756) - WireMockConsoleLogger aggregate exception handling bug? [bug]
- [#758](https://github.com/WireMock-Net/WireMock.Net/issues/758) - Add support for logging to an xUnit ITestOutputHelper [feature]

# 1.4.42 (13 May 2022)
- [#741](https://github.com/WireMock-Net/WireMock.Net/issues/741) - Integrate with Pact [feature]
- [#753](https://github.com/WireMock-Net/WireMock.Net/issues/753) - FluentAssertions - assert the server has not received a call  [feature]

# 1.4.41 (21 April 2022)
- [#744](https://github.com/WireMock-Net/WireMock.Net/issues/744) - System.ArgumentOutOfRangeException when Timeout.InfiniteTimeSpan used as an argument for IResponseBuilder.WithDelay() [bug]

# 1.4.37 (02 March 2022)
- [#726](https://github.com/WireMock-Net/WireMock.Net/issues/726) - Wiremock -  WatchStaticMappings only works until the first request is made  [bug]

# 1.4.35 (09 February 2022)
- [#721](https://github.com/WireMock-Net/WireMock.Net/issues/721) - Response BodyAsJson with array does not work [bug]

# 1.4.34 (27 January 2022)
- [#715](https://github.com/WireMock-Net/WireMock.Net/issues/715) - Record request mapping outputs JsonMatcher with Patterns instead of Pattern [bug]

# 1.4.30 (25 December 2021)
- [#534](https://github.com/WireMock-Net/WireMock.Net/issues/534) - Mock server not answer if integrated in Xamarin UITest project [bug]
- [#567](https://github.com/WireMock-Net/WireMock.Net/issues/567) - Can't start WireMock.Net server in Xamarin.UITest project (.NET Framework 4.7.2) on MacOS [bug]
- [#685](https://github.com/WireMock-Net/WireMock.Net/issues/685) - GuidWildcardMatcher to match on GUIDs [feature]

# 1.4.28 (01 December 2021)
- [#666](https://github.com/WireMock-Net/WireMock.Net/issues/666) - Example is not working as expected [bug]
- [#692](https://github.com/WireMock-Net/WireMock.Net/issues/692) - Case insensitive and ignoring optional path and header parameters in OpenApiPathsMapper [feature]

# 1.4.23 (27 September 2021)
- [#634](https://github.com/WireMock-Net/WireMock.Net/issues/634) - Upgrade to latest FluentAssertions [bug]

# 1.4.20 (06 August 2021)
- [#626](https://github.com/WireMock-Net/WireMock.Net/issues/626) - version 1.4.19 throws a lot of analyzer errors related to the BaseBuilder.cs [bug]

# 1.4.19 (04 August 2021)
- [#621](https://github.com/WireMock-Net/WireMock.Net/issues/621) - Fluent API for RestClient MappingModel creation [feature]
- [#624](https://github.com/WireMock-Net/WireMock.Net/issues/624) - Post request with &quot;BodyAsBytes&quot; is not matched by RegexMatcher [bug]

# 1.4.18 (10 July 2021)
- [#618](https://github.com/WireMock-Net/WireMock.Net/issues/618) - Trying to use attribute of the request object while creating response while mocking a soap service [bug]

# 1.4.15 (19 May 2021)
- [#614](https://github.com/WireMock-Net/WireMock.Net/issues/614) - Is it possible to some how send multiple webhooks? [feature]

# 1.4.13 (26 April 2021)
- [#608](https://github.com/WireMock-Net/WireMock.Net/issues/608) - Import from OpenApi generates model with path parameter narrowed in range (example value=42 instead of '*') [feature]

# 1.4.11 (18 April 2021)
- [#601](https://github.com/WireMock-Net/WireMock.Net/issues/601) - Exact byte array request matching fails on specific byte arrays [bug]

# 1.4.10 (15 April 2021)
- [#602](https://github.com/WireMock-Net/WireMock.Net/issues/602) - Header not being returned when set in WithCallback [bug]

# 1.4.8 (24 March 2021)
- [#589](https://github.com/WireMock-Net/WireMock.Net/issues/589) - How to send a request to a specific URL after sending response [feature]

# 1.4.6 (26 February 2021)
- [#569](https://github.com/WireMock-Net/WireMock.Net/issues/569) - WithCallback circumvent the rest of the builder [bug]

# 1.4.4 (09 February 2021)
- [#568](https://github.com/WireMock-Net/WireMock.Net/issues/568) - [Question] Dates in response templates [feature]

# 1.4.3 (05 February 2021)
- [#577](https://github.com/WireMock-Net/WireMock.Net/issues/577) - WireMock.Net will not run with certain .net5 dependencies installed in the project [bug]

# 1.4.2 (24 January 2021)
- [#565](https://github.com/WireMock-Net/WireMock.Net/issues/565) - NullReferenceException [bug]

# 1.4.1 (19 January 2021)
- [#214](https://github.com/WireMock-Net/WireMock.Net/issues/214) - Feature: Add support for template language DotLiquid [feature]

# 1.3.10 (23 December 2020)
- [#559](https://github.com/WireMock-Net/WireMock.Net/issues/559) - WireMock Setting 'SaveMappingToFile' raising cast object to type error [bug]

# 1.3.9 (08 December 2020)
- [#549](https://github.com/WireMock-Net/WireMock.Net/issues/549) - WithProxy(...) does not save the mappings to file [bug]

# 1.3.8 (03 December 2020)
- [#524](https://github.com/WireMock-Net/WireMock.Net/issues/524) - Proxying with SSL Not Working in .NET Core 3.1 [bug]

# 1.3.6 (10 November 2020)
- [#533](https://github.com/WireMock-Net/WireMock.Net/issues/533) - Stubbed response with only callback returns unexpected status code. [bug]
- [#536](https://github.com/WireMock-Net/WireMock.Net/issues/536) - Overriding the default ssl certificate via file. [feature]

# 1.3.3 (15 October 2020)
- [#521](https://github.com/WireMock-Net/WireMock.Net/issues/521) - Make Kestrel limits configurable [feature]

# 1.3.2 (14 October 2020)
- [#504](https://github.com/WireMock-Net/WireMock.Net/issues/504) - Loading mapping models with `JsonMatcher`  is not working correctly [bug]
- [#513](https://github.com/WireMock-Net/WireMock.Net/issues/513) - Static mapping break from 1.2.17 to 1.2.18 and higher [bug]

# 1.3.0 (29 September 2020)
- [#327](https://github.com/WireMock-Net/WireMock.Net/issues/327) - Index must be within the bounds of the List - Bug [bug]
- [#507](https://github.com/WireMock-Net/WireMock.Net/issues/507) - Fix vulnerability found in Microsoft.AspNetCore dependency [feature]

# 1.2.18 (13 August 2020)
- [#478](https://github.com/WireMock-Net/WireMock.Net/issues/478) - Sometimes returns status code 0 in unit tests with xunit test fixture (flaky test) [bug]

# 1.2.17 (01 August 2020)
- [#494](https://github.com/WireMock-Net/WireMock.Net/issues/494) - Stay in Current State for specified number of requests [feature]

# 1.2.16 (27 July 2020)
- [#489](https://github.com/WireMock-Net/WireMock.Net/issues/489) - Change &quot;blacklist&quot; and &quot;whitelist&quot; terms [feature]

# 1.2.14 (09 July 2020)
- [#486](https://github.com/WireMock-Net/WireMock.Net/issues/486) - Admin API fails to create a mapping with Request Body matching [bug]

# 1.2.13 (24 May 2020)
- [#474](https://github.com/WireMock-Net/WireMock.Net/issues/474) - Performance issue with multiple httpclients (since version 1.2.10) [bug]

# 1.2.12 (23 May 2020)
- [#468](https://github.com/WireMock-Net/WireMock.Net/issues/468) - Proxy mode: Incorrect handling of multipart requests [bug]

# 1.2.11.0 (18 May 2020)
- [#467](https://github.com/WireMock-Net/WireMock.Net/issues/467) - Proxy mode: Unhandled exception when target is not working [bug]

# 1.2.10 (17 May 2020)
- [#455](https://github.com/WireMock-Net/WireMock.Net/issues/455) - There is no option to increase body size while proxying [bug]

# 1.2.9.0 (14 May 2020)
- [#464](https://github.com/WireMock-Net/WireMock.Net/issues/464) - RestClient Admin API Metadata Base Path Duplication [bug]

# 1.2.7.0 (30 April 2020)
- [#459](https://github.com/WireMock-Net/WireMock.Net/issues/459) - When respond with proxy requestMessage.Url is used, not AbsoluteUrl [bug]

# 1.2.6.0 (29 April 2020)
- [#458](https://github.com/WireMock-Net/WireMock.Net/issues/458) - Response BodyAsString loses BodyData.Encoding when UseTransformer = true [bug]

# 1.2.5.0 (17 April 2020)
- [#453](https://github.com/WireMock-Net/WireMock.Net/issues/453) - MockServer not starting  [bug]

# 1.2.4.0 (10 April 2020)
- [#426](https://github.com/WireMock-Net/WireMock.Net/issues/426) - Add support for compressed requests, such as GZIP or DEFLATE [feature]

# 1.2.3.0 (01 April 2020)
- [#447](https://github.com/WireMock-Net/WireMock.Net/issues/447) - Add support for .NET Standard 2.1 / .NET Core 3.1 [feature]
- [#448](https://github.com/WireMock-Net/WireMock.Net/issues/448) - WireMock.Net is not compatible with Microsoft.VisualStudio.Web.CodeGeneration.Design 3.1.1 [bug]

# 1.2.2.0 (25 March 2020)
- [#445](https://github.com/WireMock-Net/WireMock.Net/issues/445) - Port where WireMockServer listens to - v1.2x [bug]

# 1.2.0.0 (14 March 2020)
- [#379](https://github.com/WireMock-Net/WireMock.Net/issues/379) - Trusting the self signed certificate to enable SSL on dotnet core [bug]
- [#420](https://github.com/WireMock-Net/WireMock.Net/issues/420) - Updating to 1.1.6+ breaks tests because new AllowAnyHttpStatusCodeInResponse option defaults to false [bug]

# 1.1.10 (05 March 2020)
- [#408](https://github.com/WireMock-Net/WireMock.Net/issues/408) - Intermittent threading errors with FindLogEntries [bug]
- [#433](https://github.com/WireMock-Net/WireMock.Net/issues/433) - HandlebarsRegistrationCallback not fired [feature]

# 1.1.9.0 (25 February 2020)
- [#425](https://github.com/WireMock-Net/WireMock.Net/issues/425) - Allow 64 bit numbers in JSON [bug]

# 1.1.8.0 (22 February 2020)
- [#418](https://github.com/WireMock-Net/WireMock.Net/issues/418) - Body matching fails if body has newline [bug]

# 1.1.7.0 (06 February 2020)
- [#412](https://github.com/WireMock-Net/WireMock.Net/issues/412) - WireMock Standalone - null reference exception since settings.Logger [bug]

# 1.1.3.0 (22 January 2020)
- [#402](https://github.com/WireMock-Net/WireMock.Net/issues/402) - Invalid Cast Exception  [bug]

# 1.1.2.0 (09 January 2020)
- [#400](https://github.com/WireMock-Net/WireMock.Net/issues/400) - StatusCode not built correctly when loaded from mapping file. [bug]

# 1.1.1.0 (09 January 2020)
- [#397](https://github.com/WireMock-Net/WireMock.Net/issues/397) - Question/Feature: Add support for selecting XPath in response template [feature]

# 1.0.43.0 (26 December 2019)
- [#380](https://github.com/WireMock-Net/WireMock.Net/issues/380) - StatusCode is defined as integer (string is not possible) [bug]
- [#382](https://github.com/WireMock-Net/WireMock.Net/issues/382) - Return same request body [feature]

# 1.0.42.0 (15 December 2019)
- [#383](https://github.com/WireMock-Net/WireMock.Net/issues/383) - ExactMatcher does not accept ISO8601 DateTime? [bug]

# 1.0.41.0 (14 December 2019)
- [#390](https://github.com/WireMock-Net/WireMock.Net/issues/390) - JsonMatcher does not match a body containing an array of strings [bug]

# 1.0.40.0 (09 December 2019)
- [#387](https://github.com/WireMock-Net/WireMock.Net/issues/387) - Query string parameter value which contains %26 does not work with ExactMatcher [bug]

# 1.0.39.0 (07 December 2019)
- [#369](https://github.com/WireMock-Net/WireMock.Net/issues/369) - Question: Is there a way to provide a corporate proxy configuration? [feature]
- [#375](https://github.com/WireMock-Net/WireMock.Net/issues/375) - Proxying does not follow redirects : make this configurable [feature]
- [#386](https://github.com/WireMock-Net/WireMock.Net/issues/386) - Is transforming contents of XML file supported.? [bug]

# 1.0.38.0 (30 November 2019)
- [#377](https://github.com/WireMock-Net/WireMock.Net/issues/377) - Unable to build against .NET 4.5.1 because of Handlebars [bug]

# 1.0.37.0 (08 November 2019)
- [#372](https://github.com/WireMock-Net/WireMock.Net/issues/372) - Reset in WireMock admin API not working fine. [feature]

# 1.0.36.0 (26 October 2019)
- [#343](https://github.com/WireMock-Net/WireMock.Net/issues/343) - Feature: Please provide support for Bad responses. [feature]

# 1.0.34.0 (22 October 2019)
- [#352](https://github.com/WireMock-Net/WireMock.Net/issues/352) - DELETE request drops the body [feature]

# 1.0.33.0 (12 October 2019)
- [#306](https://github.com/WireMock-Net/WireMock.Net/issues/306) - Writing to the response body is invalid for responses with status code 204 [bug]
- [#307](https://github.com/WireMock-Net/WireMock.Net/issues/307) - JsonPathMatcher always convert to JArray before matching [bug]
- [#329](https://github.com/WireMock-Net/WireMock.Net/issues/329) - Feature: Add support for CSharpCodeMatcher [feature]
- [#350](https://github.com/WireMock-Net/WireMock.Net/issues/350) - Admin requests fail when content type includes a charset [bug]
- [#356](https://github.com/WireMock-Net/WireMock.Net/issues/356) - JsonMatcher not working when JSON contains a DateTimeOffset [bug]

# 1.0.32.0 (20 September 2019)
- [#347](https://github.com/WireMock-Net/WireMock.Net/issues/347) - Query string match on DateTimeOffset is not working [bug]

# 1.0.31.0 (19 September 2019)
- [#337](https://github.com/WireMock-Net/WireMock.Net/issues/337) - Proxy Missing header Content-Type - tried with Recording [bug]
- [#344](https://github.com/WireMock-Net/WireMock.Net/issues/344) - Mapping adding order matters for multiple mappings? [bug]

# 1.0.29.0 (29 August 2019)
- [#332](https://github.com/WireMock-Net/WireMock.Net/issues/332) - Case sensitive true is ignored for JsonMatcher [feature]

# 1.0.28.0 (20 August 2019)
- [#252](https://github.com/WireMock-Net/WireMock.Net/issues/252) - Proxy with Transform
- [#308](https://github.com/WireMock-Net/WireMock.Net/issues/308) - __admin/requests - &quot;Collection was modified&quot; exception [bug]
- [#313](https://github.com/WireMock-Net/WireMock.Net/issues/313) - RequestLogExpirationDuration - bug [bug]
- [#325](https://github.com/WireMock-Net/WireMock.Net/issues/325) - Admin API: PUT Mapping, FormatException because of wrong parsing of the Query  [bug]

# 1.0.24.0 (22 July 2019)
- [#301](https://github.com/WireMock-Net/WireMock.Net/issues/301) - Error thrown when calling mocked endpoint second time when using file-based response body [bug]

# 1.0.22.0 (15 July 2019)
- [#295](https://github.com/WireMock-Net/WireMock.Net/issues/295) - NullRef in 1.0.21 [bug]

# 1.0.21.0 (03 July 2019)
- [#289](https://github.com/WireMock-Net/WireMock.Net/issues/289) - Bug:  When WatchStaticMappings=true throws exceptions on updating the mapping files [bug]
- [#290](https://github.com/WireMock-Net/WireMock.Net/issues/290) - Request body is dropped if verb is REPORT [bug]
- [#292](https://github.com/WireMock-Net/WireMock.Net/issues/292) - Can't start server in Xamarin Android [bug]

# 1.0.19.0 (15 June 2019)
- [#283](https://github.com/WireMock-Net/WireMock.Net/issues/283) - Support equal-sign in query [bug]

# 1.0.16.0 (16 May 2019)
- [#160](https://github.com/WireMock-Net/WireMock.Net/issues/160) - Feature: Sign 'WireMock.Net' [feature]
- [#267](https://github.com/WireMock-Net/WireMock.Net/issues/267) - Assembly does not have strong name

# 1.0.13.0 (11 April 2019)
- [#265](https://github.com/WireMock-Net/WireMock.Net/issues/265) - File Upload [feature]

# 1.0.12.0 (05 April 2019)
- [#263](https://github.com/WireMock-Net/WireMock.Net/issues/263) - Content-Type multipart/form-data is not serialized in proxy and recording mode [bug]

# 1.0.9.0 (25 March 2019)
- [#255](https://github.com/WireMock-Net/WireMock.Net/issues/255) - ExactMatcher with array pattern not working? [bug]

# 1.0.8.0 (12 March 2019)
- [#253](https://github.com/WireMock-Net/WireMock.Net/issues/253) - Request Path and query parameter keys are case-sensitive

# 1.0.7.0 (19 January 2019)
- [#240](https://github.com/WireMock-Net/WireMock.Net/issues/240) - How to submit mappings for multiple request, responses [feature]
- [#243](https://github.com/WireMock-Net/WireMock.Net/issues/243) - Not able to read response from file [bug]

# 1.0.6.1 (10 January 2019)
- [#225](https://github.com/WireMock-Net/WireMock.Net/issues/225) - Feature: Improve logging in example for WireMock as Windows Service [feature]
- [#248](https://github.com/WireMock-Net/WireMock.Net/issues/248) - Content-Type multipart/form-data is not seen as byte[] anymore

# 1.0.4.21 (30 November 2018)
- [#219](https://github.com/WireMock-Net/WireMock.Net/issues/219) - Feature: random value helper [feature]
- [#234](https://github.com/WireMock-Net/WireMock.Net/issues/234) - Timeout Exception on VSTS Test Platform (Azure DevOps), with private build agent

# 1.0.4.20 (07 November 2018)
- [#223](https://github.com/WireMock-Net/WireMock.Net/issues/223) - Bug: Example for WireMock as Windows Service throws Exception because of WireMockConsoleLogger [bug]

# 1.0.4.18 (27 October 2018)
- [#107](https://github.com/WireMock-Net/WireMock.Net/issues/107) - Feature: increase code coverage [feature]
- [#161](https://github.com/WireMock-Net/WireMock.Net/issues/161) - Feature: Implement SourceLink [feature]
- [#179](https://github.com/WireMock-Net/WireMock.Net/issues/179) - BodyAsFile .json files interferes with WatchStaticMappings
- [#194](https://github.com/WireMock-Net/WireMock.Net/issues/194) - Could not load file or assembly 'netstandard, Version=2.0.0.0 On Build Server
- [#206](https://github.com/WireMock-Net/WireMock.Net/issues/206) - Rewrite some unit-integration-tests to unit-tests
- [#210](https://github.com/WireMock-Net/WireMock.Net/issues/210) - When proxying, the Content-Type headers get dropped from the request
- [#211](https://github.com/WireMock-Net/WireMock.Net/issues/211) - Feature: Add support to recognise custom json media-types
- [#213](https://github.com/WireMock-Net/WireMock.Net/issues/213) - Question: Unable get response from wiremock.net server in c#
- [#215](https://github.com/WireMock-Net/WireMock.Net/issues/215) - Issue: upgrade Microsoft.AspNetCore / Microsoft.AspNetCore.All to 2.1.5

# 1.0.4.17 (22 September 2018)
- [#200](https://github.com/WireMock-Net/WireMock.Net/issues/200) - Issue: Incorrect port matching
- [#205](https://github.com/WireMock-Net/WireMock.Net/issues/205) - Issue: DELETE method is proxied as lowercase [bug]

# 1.0.4.16 (11 September 2018)
- [#201](https://github.com/WireMock-Net/WireMock.Net/issues/201) - Question : Extracting text from a request.body that is not json

# 1.0.4.15 (04 September 2018)
- [#198](https://github.com/WireMock-Net/WireMock.Net/issues/198) - Issue : creating response using .WithBody(Func&lt;RequestMessage, string&gt;...) and .WithStatusCode [bug]

# 1.0.4.14 (02 September 2018)
- [#196](https://github.com/WireMock-Net/WireMock.Net/issues/196) - Issue: AspNetCoreSelfHost.IsStarted set before the server actually started for real [bug]

# 1.0.4.13 (31 August 2018)
- [#192](https://github.com/WireMock-Net/WireMock.Net/issues/192) - Cannot upgrade from 1.0.4.10 to 1.0.4.12 without upgrading to .net core 2.1 [bug]

# 1.0.4.12 (23 August 2018)
- [#188](https://github.com/WireMock-Net/WireMock.Net/issues/188) - Bug: ResponseMessageTransformer : 
- [#189](https://github.com/WireMock-Net/WireMock.Net/issues/189) - Issue: Case of header key/name not ignored in RequestBuilder when ignoreCase == true

# 1.0.4.11 (20 August 2018)
- [#182](https://github.com/WireMock-Net/WireMock.Net/issues/182) - Bug: IFluentMockServerAdmin::PutMappingAsync does not set Content-Type
- [#184](https://github.com/WireMock-Net/WireMock.Net/issues/184) - Bug: Fix AppVeyor PR build process
- [#187](https://github.com/WireMock-Net/WireMock.Net/issues/187) - Bug: Admin GetRequestAsync does not populate request body for JsonApi (&quot;application/vnd.api+json&quot;) content

# 1.0.4.10 (14 August 2018)
- [#173](https://github.com/WireMock-Net/WireMock.Net/issues/173) - Feature: Mapping files lost when restarting an Azure app service [feature]

# 1.0.4.9 (08 August 2018)
- [#172](https://github.com/WireMock-Net/WireMock.Net/issues/172) - Question: Same/similar fluent interface for in process and admin client API
- [#174](https://github.com/WireMock-Net/WireMock.Net/issues/174) - Bug: JsonMatcher and JsonPathMatcher throws when posting byte[] [bug]
- [#175](https://github.com/WireMock-Net/WireMock.Net/issues/175) - Bug: Don't allow adding a mapping with no URL or PATH [bug]
- [#176](https://github.com/WireMock-Net/WireMock.Net/issues/176) - Question: Saving mapping with relative (not found) file fails
- [#177](https://github.com/WireMock-Net/WireMock.Net/issues/177) - Feature: Skip invalid static mapping files [feature]

# 1.0.4.8 (23 July 2018)
- [#167](https://github.com/WireMock-Net/WireMock.Net/issues/167) - Feature: Support for JsonPath in the response (with HandleBars) [feature]

# 1.0.4.7 (19 July 2018)
- [#148](https://github.com/WireMock-Net/WireMock.Net/issues/148) - Question: proxy passthrough when no match?

# 1.0.4.6 (18 July 2018)
- [#163](https://github.com/WireMock-Net/WireMock.Net/issues/163) - Feature: Expose scenario states

# 1.0.4.5 (17 July 2018)
- [#120](https://github.com/WireMock-Net/WireMock.Net/issues/120) - Question: JsonPathMatcher - not matching? Correct syntax?
- [#123](https://github.com/WireMock-Net/WireMock.Net/issues/123) - Fix for DateTime Header causing null value in ResponseBuilder

# 1.0.4.4 (01 July 2018)
- [#156](https://github.com/WireMock-Net/WireMock.Net/issues/156) - Feature: when adding / updating a mapping : return more details

# 1.0.4.3 (30 June 2018)
- [#159](https://github.com/WireMock-Net/WireMock.Net/issues/159) - Bug: IRequestBuilder.WithParam broken for key-only matching [bug]

# 1.0.4.2 (26 June 2018)
- [#150](https://github.com/WireMock-Net/WireMock.Net/issues/150) - Add support for .NET Core 2.1 (.NET Core 2.0 will reach end of life on september 2018)
- [#154](https://github.com/WireMock-Net/WireMock.Net/issues/154) - Feature: support BodyAsJson for Request in static mapping files. [feature]

# 1.0.4.1 (25 June 2018)
- [#153](https://github.com/WireMock-Net/WireMock.Net/issues/153) - Feature: Add JsonMatcher to support Json mapping

# 1.0.4.0 (23 June 2018)
- [#131](https://github.com/WireMock-Net/WireMock.Net/issues/131) - Bug: CurlException Couldn't connect to Server when running multiple tests
- [#149](https://github.com/WireMock-Net/WireMock.Net/issues/149) - Question: Transformer and Delay in Static Mappings?
- [#151](https://github.com/WireMock-Net/WireMock.Net/issues/151) - Feature: Add logging of incoming request and body for tracability

# 1.0.3.20 (29 May 2018)
- [#129](https://github.com/WireMock-Net/WireMock.Net/issues/129) - Random test failures between WireMock.Net 1.0.3.1 and 1.0.3.2
- [#146](https://github.com/WireMock-Net/WireMock.Net/issues/146) - Hang possibly due to Windows firewall prompt

# 1.0.3.18 (25 May 2018)
- [#122](https://github.com/WireMock-Net/WireMock.Net/issues/122) - WireMock.Net not responding in unit tests - same works in console application
- [#126](https://github.com/WireMock-Net/WireMock.Net/issues/126) - Question: UsingHead always returns 0 for Content-Length header even when explicitly specified
- [#132](https://github.com/WireMock-Net/WireMock.Net/issues/132) - LogEntries not being recorded on subsequent tests
- [#136](https://github.com/WireMock-Net/WireMock.Net/issues/136) - Question: Does the WireMock send Content-Length response header
- [#137](https://github.com/WireMock-Net/WireMock.Net/issues/137) - Question: How to specify Transfer-Encoding response header?
- [#139](https://github.com/WireMock-Net/WireMock.Net/issues/139) - Wiki link https://github.com/StefH/WireMock.Net/wiki/Record-(via-proxy)-and-Save is dead

# 1.0.3.17 (16 May 2018)
- [#130](https://github.com/WireMock-Net/WireMock.Net/issues/130) - ...

# 1.0.3.16 (17 April 2018)
- [#118](https://github.com/WireMock-Net/WireMock.Net/issues/118) - Not reading the response from a file when mappings are placed in json file
- [#124](https://github.com/WireMock-Net/WireMock.Net/issues/124) - Issue: Unable to get host to listen on ips other than 127.0.0.1 using StandAloneApp

# 1.0.3.14 (01 April 2018)
- [#111](https://github.com/WireMock-Net/WireMock.Net/issues/111) - Question: Adding wiki documentation on how to use WireMock.Net.WebApplication project
- [#112](https://github.com/WireMock-Net/WireMock.Net/issues/112) - Question: Request.Create().WithBody() not able to match with custom class which implements IMatcher
- [#113](https://github.com/WireMock-Net/WireMock.Net/issues/113) - Feature: Add BodyAsJsonIndented for response message [feature]
- [#114](https://github.com/WireMock-Net/WireMock.Net/issues/114) - Feature: Add PathSegments in Transform [feature]

# 1.0.3.11 (20 March 2018)
- [#110](https://github.com/WireMock-Net/WireMock.Net/issues/110) - Fix: remove `Func[]` from MappingModel

# 1.0.3.10 (17 March 2018)
- [#109](https://github.com/WireMock-Net/WireMock.Net/issues/109) - Issue: When proxying, MimeType is wrong for StringContent

# 1.0.3.9 (15 March 2018)
- [#108](https://github.com/WireMock-Net/WireMock.Net/issues/108) - Issue: provide correct contentTypeHeader value for the bodyparser [bug]

# 1.0.3.8 (10 March 2018)
- [#106](https://github.com/WireMock-Net/WireMock.Net/issues/106) - Issue: Params does not work, when there are multiple values for a key

# 1.0.3.4 (04 March 2018)
- [#66](https://github.com/WireMock-Net/WireMock.Net/issues/66) - Interested in callbacks?
- [#93](https://github.com/WireMock-Net/WireMock.Net/issues/93) - Bug: FluentMockServer IsStarted after calling Start()
- [#94](https://github.com/WireMock-Net/WireMock.Net/issues/94) - Issue: Introduced dependency on log4net
- [#98](https://github.com/WireMock-Net/WireMock.Net/issues/98) - IBodyResponseBuilder.WithBody* should receive the request as a parameter

# 1.0.3.3 (24 February 2018)
- [#91](https://github.com/WireMock-Net/WireMock.Net/issues/91) - Bug: WireMock.Net is not matching application/json http requests using JSONPathMatcher [bug]

# 1.0.3.2 (14 February 2018)
- [#88](https://github.com/WireMock-Net/WireMock.Net/issues/88) - Bug: Standalone server throws 500 error when receiving concurrent requests [bug]

# 1.0.3.1 (14 February 2018)
- [#87](https://github.com/WireMock-Net/WireMock.Net/issues/87) - Feature: Add logging

# 1.0.3.0 (05 February 2018)
- [#80](https://github.com/WireMock-Net/WireMock.Net/issues/80) - Feature: When using proxy, in case Content-Type is JSON, use BodyAsJson in Response
- [#81](https://github.com/WireMock-Net/WireMock.Net/issues/81) - Feature: When using proxy, only BodyAsBytes in case of binary data?
- [#82](https://github.com/WireMock-Net/WireMock.Net/issues/82) - Feature: make it possible to ignore some headers when proxying [feature]
- [#83](https://github.com/WireMock-Net/WireMock.Net/issues/83) - Feature : Add also a method in IProxyResponseBuilder to provide proxy-settings [feature]
- [#85](https://github.com/WireMock-Net/WireMock.Net/issues/85) - Bug: https for netstandard does not work ? [bug]
- [#86](https://github.com/WireMock-Net/WireMock.Net/issues/86) - Feature : Add FileSystemWatcher logic for watching static mapping files [feature]

# 1.0.2.13 (23 January 2018)
- [#57](https://github.com/WireMock-Net/WireMock.Net/issues/57) - ProxyAndRecord does not save query-parameters, headers and body [bug]
- [#78](https://github.com/WireMock-Net/WireMock.Net/issues/78) - WireMock not working when attempting to access from anything other than localhost.

# 1.0.2.12 (16 January 2018)
- [#73](https://github.com/WireMock-Net/WireMock.Net/issues/73) - Updated mapping is not being picked and responded with the response
- [#76](https://github.com/WireMock-Net/WireMock.Net/issues/76) - Bug: IFluentMockServerAdmin is missing content-type for some POST/PUT calls

# 1.0.2.11 (20 December 2017)
- [#72](https://github.com/WireMock-Net/WireMock.Net/issues/72) - Matching WithParam on OData End Points

# 1.0.2.10 (12 December 2017)
- [#70](https://github.com/WireMock-Net/WireMock.Net/issues/70) - Proxy/Intercept pattern is throwing a keep alive header error with net461

# 1.0.2.9 (07 December 2017)
- [#69](https://github.com/WireMock-Net/WireMock.Net/issues/69) - Instructions are incorrect (?)

# 1.0.2.8 (23 November 2017)
- [#64](https://github.com/WireMock-Net/WireMock.Net/issues/64) - Pull Requests do not trigger test + codecoverage ?

# 1.0.2.7 (18 November 2017)
- [#27](https://github.com/WireMock-Net/WireMock.Net/issues/27) - New feature: Record and Save
- [#42](https://github.com/WireMock-Net/WireMock.Net/issues/42) - Enhancement - Save/load request logs to/from disk [feature]
- [#53](https://github.com/WireMock-Net/WireMock.Net/issues/53) - New feature request: Access to Owin pipeline

# 1.0.2.6 (30 October 2017)
- [#54](https://github.com/WireMock-Net/WireMock.Net/issues/54) - Proxy for AWS: Error unmarshalling response back from AWS [bug]
- [#56](https://github.com/WireMock-Net/WireMock.Net/issues/56) - WithBodyFromFile Support [feature]
- [#58](https://github.com/WireMock-Net/WireMock.Net/issues/58) - Multiple headers with same name [feature]

# 1.0.2.5 (24 October 2017)
- [#44](https://github.com/WireMock-Net/WireMock.Net/issues/44) - Bug: Server not listening after Start() returns (on macOS) [bug]
- [#48](https://github.com/WireMock-Net/WireMock.Net/issues/48) - Stateful support [feature]
- [#52](https://github.com/WireMock-Net/WireMock.Net/issues/52) - SimMetrics.NET error when trying to install NuGet Package

# 1.0.2.4 (10 October 2017)
- [#15](https://github.com/WireMock-Net/WireMock.Net/issues/15) - New feature: Proxying [feature]
- [#20](https://github.com/WireMock-Net/WireMock.Net/issues/20) - Add client certificate authentication [feature]
- [#31](https://github.com/WireMock-Net/WireMock.Net/issues/31) - Feature request: Nuget package for standalone version [feature]
- [#33](https://github.com/WireMock-Net/WireMock.Net/issues/33) - Issue with launching sample code (StandAlone server) [bug]
- [#38](https://github.com/WireMock-Net/WireMock.Net/issues/38) - Bug: support also listening on *:{port}
- [#43](https://github.com/WireMock-Net/WireMock.Net/issues/43) - Feature: Add RequestLogExpirationDuration and MaxRequestLogCount
- [#46](https://github.com/WireMock-Net/WireMock.Net/issues/46) - Log the ip-address from the client/caller also in the RequestLog [feature]
- [#47](https://github.com/WireMock-Net/WireMock.Net/issues/47) - Feature: add matcher details to Request to see which matchers match/not match [feature]
- [#50](https://github.com/WireMock-Net/WireMock.Net/issues/50) - New Feature: Callbacks

# 1.0.2.1 (14 June 2017)
- [#30](https://github.com/WireMock-Net/WireMock.Net/issues/30) - [Feature] Disable partial mappings by default in standalone version [bug, feature]

# 1.0.2.0 (05 May 2017)
- [#21](https://github.com/WireMock-Net/WireMock.Net/issues/21) - Admin static json mappings [feature]
- [#23](https://github.com/WireMock-Net/WireMock.Net/issues/23) - Consider port to .Net Core
- [#25](https://github.com/WireMock-Net/WireMock.Net/issues/25) - Upgrade to vs2017 [feature]

# 1.0.1.2 (27 February 2017)
- [#8](https://github.com/WireMock-Net/WireMock.Net/issues/8) - admin rest api
- [#22](https://github.com/WireMock-Net/WireMock.Net/issues/22) - Add basic-authentication for accessing admin-interface [feature]

# 1.0.1.1 (10 February 2017)
- [#1](https://github.com/WireMock-Net/WireMock.Net/issues/1) - Replace WildcardPatternMatcher by RegEx [feature]
- [#2](https://github.com/WireMock-Net/WireMock.Net/issues/2) - Func&lt;string&gt; matching [feature]
- [#3](https://github.com/WireMock-Net/WireMock.Net/issues/3) - WithUrls and WithHeaders and ...
- [#4](https://github.com/WireMock-Net/WireMock.Net/issues/4) - Handlebar support
- [#5](https://github.com/WireMock-Net/WireMock.Net/issues/5) - Xml(2)Path matching
- [#6](https://github.com/WireMock-Net/WireMock.Net/issues/6) - JsonPath support matching
- [#9](https://github.com/WireMock-Net/WireMock.Net/issues/9) - Cookie matching
- [#10](https://github.com/WireMock-Net/WireMock.Net/issues/10) - Add usingDelete [feature]
- [#11](https://github.com/WireMock-Net/WireMock.Net/issues/11) - Add response body in binary format  [feature]
- [#12](https://github.com/WireMock-Net/WireMock.Net/issues/12) - Getting all currently registered stub mappings [feature]
- [#13](https://github.com/WireMock-Net/WireMock.Net/issues/13) - Handle Exception
- [#14](https://github.com/WireMock-Net/WireMock.Net/issues/14) - Allow Body as Base64
- [#16](https://github.com/WireMock-Net/WireMock.Net/issues/16) - Stub priority [feature]
- [#17](https://github.com/WireMock-Net/WireMock.Net/issues/17) - Add JsonBody to response [feature]
- [#18](https://github.com/WireMock-Net/WireMock.Net/issues/18) - Listen on more ip-address/ports [feature]

