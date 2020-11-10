# 1.3.6 (10 November 2020)
- [#529](https://github.com/WireMock-Net/WireMock.Net/pull/529) - Add assertions for ClientIP, Url and ProxyUrl [feature] contributed by [akamud](https://github.com/akamud)
- [#535](https://github.com/WireMock-Net/WireMock.Net/pull/535) - WithCallback should use also use enum HttpStatusCode [bug] contributed by [StefH](https://github.com/StefH)
- [#537](https://github.com/WireMock-Net/WireMock.Net/pull/537) - Add Custom Certificate settings [feature] contributed by [StefH](https://github.com/StefH)
- [#533](https://github.com/WireMock-Net/WireMock.Net/issues/533) - Stubbed response with only callback returns unexpected status code. [bug]
- [#536](https://github.com/WireMock-Net/WireMock.Net/issues/536) - Overriding the default ssl certificate via file. [feature]

# 1.3.5 (04 November 2020)
- [#530](https://github.com/WireMock-Net/WireMock.Net/pull/530) - Fix dotnet-sonarscanner [bug] contributed by [StefH](https://github.com/StefH)
- [#531](https://github.com/WireMock-Net/WireMock.Net/pull/531) - Add WithCallback-Async [feature] contributed by [StefH](https://github.com/StefH)

# 1.3.4 (17 October 2020)
- [#522](https://github.com/WireMock-Net/WireMock.Net/pull/522) - Add ContinuousIntegrationBuild property [feature] contributed by [StefH](https://github.com/StefH)
- [#525](https://github.com/WireMock-Net/WireMock.Net/pull/525) - Handlebars.Net.Helpers Version=&quot;1.1.0&quot; [feature] contributed by [StefH](https://github.com/StefH)

# 1.3.3 (15 October 2020)
- [#520](https://github.com/WireMock-Net/WireMock.Net/pull/520) - Make kestrel limits configurable contributed by [eduherminio](https://github.com/eduherminio)
- [#521](https://github.com/WireMock-Net/WireMock.Net/issues/521) - Make Kestrel limits configurable [feature]

# 1.3.2 (14 October 2020)
- [#505](https://github.com/WireMock-Net/WireMock.Net/pull/505) - Fix reading JsonMatcher-mapping with object as pattern [bug] contributed by [StefH](https://github.com/StefH)
- [#514](https://github.com/WireMock-Net/WireMock.Net/pull/514) - Update .NET Core 3.1 example contributed by [Crossbow78](https://github.com/Crossbow78)
- [#504](https://github.com/WireMock-Net/WireMock.Net/issues/504) - Loading mapping models with `JsonMatcher`  is not working correctly [bug]
- [#513](https://github.com/WireMock-Net/WireMock.Net/issues/513) - Static mapping break from 1.2.17 to 1.2.18 and higher [bug]

# 1.3.1 (30 September 2020)
- [#509](https://github.com/WireMock-Net/WireMock.Net/pull/509) - Adding netcoreapp3.1 as a target framework [feature] contributed by [APIWT](https://github.com/APIWT)

# 1.3.0 (29 September 2020)
- [#508](https://github.com/WireMock-Net/WireMock.Net/pull/508) - Fix vulnerability in NuGet dependencies contributed by [StefH](https://github.com/StefH)
- [#327](https://github.com/WireMock-Net/WireMock.Net/issues/327) - Index must be within the bounds of the List - Bug [bug]
- [#507](https://github.com/WireMock-Net/WireMock.Net/issues/507) - Fix vulnerability found in Microsoft.AspNetCore dependency [feature]

# 1.2.18 (13 August 2020)
- [#496](https://github.com/WireMock-Net/WireMock.Net/pull/496) - Add setting to handle requests synchronously [feature] contributed by [StefH](https://github.com/StefH)
- [#500](https://github.com/WireMock-Net/WireMock.Net/pull/500) - Add ThrowExceptionWhenMatcherFails option to all Matchers [feature] contributed by [StefH](https://github.com/StefH)
- [#478](https://github.com/WireMock-Net/WireMock.Net/issues/478) - Sometimes returns status code 0 in unit tests with xunit test fixture (flaky test) [bug]

# 1.2.17 (01 August 2020)
- [#495](https://github.com/WireMock-Net/WireMock.Net/pull/495) - Scenario : stay on current state for a number of times contributed by [StefH](https://github.com/StefH)
- [#494](https://github.com/WireMock-Net/WireMock.Net/issues/494) - Stay in Current State for specified number of requests [feature]

# 1.2.16 (27 July 2020)
- [#492](https://github.com/WireMock-Net/WireMock.Net/pull/492) - Mark FluentMockServer, FluentMockServerSettings, BlacklistedHeaders and BlacklistedCookies as obsolete [feature] contributed by [StefH](https://github.com/StefH)
- [#489](https://github.com/WireMock-Net/WireMock.Net/issues/489) - Change &quot;blacklist&quot; and &quot;whitelist&quot; terms [feature]

# 1.2.15 (19 July 2020)
- [#485](https://github.com/WireMock-Net/WireMock.Net/pull/485) - Add fluent assertions for headers [test] contributed by [akamud](https://github.com/akamud)

# 1.2.14 (09 July 2020)
- [#479](https://github.com/WireMock-Net/WireMock.Net/pull/479) - An OpenApi (swagger) parser to generate MappingModel or mapping.json file [feature] contributed by [StefH](https://github.com/StefH)
- [#482](https://github.com/WireMock-Net/WireMock.Net/pull/482) - Add PartialMatch to logging / logentries [feature] contributed by [StefH](https://github.com/StefH)
- [#483](https://github.com/WireMock-Net/WireMock.Net/pull/483) - Bring in the WireMock.Net.FluentAssertions tests contributed by [akamud](https://github.com/akamud)
- [#484](https://github.com/WireMock-Net/WireMock.Net/pull/484) - Refactor: extract interfaces [feature] contributed by [StefH](https://github.com/StefH)
- [#487](https://github.com/WireMock-Net/WireMock.Net/pull/487) - Fixed MappingConverter when methods are null [bug] contributed by [StefH](https://github.com/StefH)
- [#486](https://github.com/WireMock-Net/WireMock.Net/issues/486) - Admin API fails to create a mapping with Request Body matching [bug]

# 1.2.13 (24 May 2020)
- [#475](https://github.com/WireMock-Net/WireMock.Net/pull/475) - Fix Limits.KeepAliveTimeout &amp; Limits.RequestHeadersTimeout [bug] contributed by [StefH](https://github.com/StefH)
- [#474](https://github.com/WireMock-Net/WireMock.Net/issues/474) - Performance issue with multiple httpclients (since version 1.2.10) [bug]

# 1.2.12 (23 May 2020)
- [#472](https://github.com/WireMock-Net/WireMock.Net/pull/472) - Create new .sln contributed by [StefH](https://github.com/StefH)
- [#473](https://github.com/WireMock-Net/WireMock.Net/pull/473) - Fixed Proxy when using MultipartForm with byte[] [bug] contributed by [StefH](https://github.com/StefH)
- [#468](https://github.com/WireMock-Net/WireMock.Net/issues/468) - Proxy mode: Incorrect handling of multipart requests [bug]

# 1.2.11.0 (18 May 2020)
- [#469](https://github.com/WireMock-Net/WireMock.Net/pull/469) - Fix unhandled exception when target is unavailable [bug] contributed by [StefH](https://github.com/StefH)
- [#467](https://github.com/WireMock-Net/WireMock.Net/issues/467) - Proxy mode: Unhandled exception when target is not working [bug]

# 1.2.10 (17 May 2020)
- [#456](https://github.com/WireMock-Net/WireMock.Net/pull/456) - Include Handlebars.Net.Helpers project [feature] contributed by [StefH](https://github.com/StefH)
- [#457](https://github.com/WireMock-Net/WireMock.Net/pull/457) - Kestrel Options Limits [bug] contributed by [StefH](https://github.com/StefH)
- [#455](https://github.com/WireMock-Net/WireMock.Net/issues/455) - There is no option to increase body size while proxying [bug]

# 1.2.9.0 (14 May 2020)
- [#465](https://github.com/WireMock-Net/WireMock.Net/pull/465) - Fix method ResetMappingsAsync in the RestEase-AdminApi [bug] contributed by [StefH](https://github.com/StefH)
- [#464](https://github.com/WireMock-Net/WireMock.Net/issues/464) - RestClient Admin API Metadata Base Path Duplication [bug]

# 1.2.8.0 (04 May 2020)
- [#463](https://github.com/WireMock-Net/WireMock.Net/pull/463) - GH161: Fix SourceLink support [bug] contributed by [gitfool](https://github.com/gitfool)

# 1.2.7.0 (30 April 2020)
- [#461](https://github.com/WireMock-Net/WireMock.Net/pull/461) - Support Path in ProxyUrl contributed by [StefH](https://github.com/StefH)
- [#459](https://github.com/WireMock-Net/WireMock.Net/issues/459) - When respond with proxy requestMessage.Url is used, not AbsoluteUrl [bug]

# 1.2.6.0 (29 April 2020)
- [#460](https://github.com/WireMock-Net/WireMock.Net/pull/460) - When using ResponseMessageTransformer : keep BodyEncoding [bug] contributed by [StefH](https://github.com/StefH)
- [#458](https://github.com/WireMock-Net/WireMock.Net/issues/458) - Response BodyAsString loses BodyData.Encoding when UseTransformer = true [bug]

# 1.2.5.0 (17 April 2020)
- [#454](https://github.com/WireMock-Net/WireMock.Net/pull/454) - Fix port = 0 for net452 [bug] contributed by [StefH](https://github.com/StefH)
- [#453](https://github.com/WireMock-Net/WireMock.Net/issues/453) - MockServer not starting  [bug]

# 1.2.4.0 (10 April 2020)
- [#439](https://github.com/WireMock-Net/WireMock.Net/pull/439) - Add support for GZip and Deflate [feature] contributed by [StefH](https://github.com/StefH)
- [#444](https://github.com/WireMock-Net/WireMock.Net/pull/444) - Add readme.md + license from mock4net [feature] contributed by [StefH](https://github.com/StefH)
- [#451](https://github.com/WireMock-Net/WireMock.Net/pull/451) - Update NuGet dependencies (e.g. coverage related) to fix CI-build [feature] contributed by [StefH](https://github.com/StefH)
- [#452](https://github.com/WireMock-Net/WireMock.Net/pull/452) - Add ValidatedNotNullAttribute (for SonarQube) [refactor] contributed by [StefH](https://github.com/StefH)
- [#426](https://github.com/WireMock-Net/WireMock.Net/issues/426) - Add support for compressed requests, such as GZIP or DEFLATE [feature]

# 1.2.3.0 (01 April 2020)
- [#449](https://github.com/WireMock-Net/WireMock.Net/pull/449) - Netstandard21 [feature] contributed by [StefH](https://github.com/StefH)
- [#447](https://github.com/WireMock-Net/WireMock.Net/issues/447) - Add support for .NET Standard 2.1 / .NET Core 3.1 [feature]
- [#448](https://github.com/WireMock-Net/WireMock.Net/issues/448) - WireMock.Net is not compatible with Microsoft.VisualStudio.Web.CodeGeneration.Design 3.1.1 [bug]

# 1.2.2.0 (25 March 2020)
- [#446](https://github.com/WireMock-Net/WireMock.Net/pull/446) - When port is provided: WireMockServer still takes a random port [bug] contributed by [StefH](https://github.com/StefH)
- [#445](https://github.com/WireMock-Net/WireMock.Net/issues/445) - Port where WireMockServer listens to - v1.2x [bug]

# 1.2.1.0 (17 March 2020)
- [#442](https://github.com/WireMock-Net/WireMock.Net/pull/442) - Fix Null body in handlebars transformation [bug] contributed by [StefH](https://github.com/StefH)

# 1.2.0.0 (14 March 2020)
- [#417](https://github.com/WireMock-Net/WireMock.Net/pull/417) - Let the .NET core/standard WebHostBuilder use a random port [bug] contributed by [StefH](https://github.com/StefH)
- [#422](https://github.com/WireMock-Net/WireMock.Net/pull/422) - AllowOnlyDefinedHttpStatusCodeInResponse [bug] contributed by [StefH](https://github.com/StefH)
- [#379](https://github.com/WireMock-Net/WireMock.Net/issues/379) - Trusting the self signed certificate to enable SSL on dotnet core [bug]
- [#420](https://github.com/WireMock-Net/WireMock.Net/issues/420) - Updating to 1.1.6+ breaks tests because new AllowAnyHttpStatusCodeInResponse option defaults to false [bug]

# 1.1.10 (05 March 2020)
- [#427](https://github.com/WireMock-Net/WireMock.Net/pull/427) - Add UsingOptions, UsingConnect and UsingTrace [feature] contributed by [StefH](https://github.com/StefH)
- [#434](https://github.com/WireMock-Net/WireMock.Net/pull/434) - Option to disable JSON deserialization [feature] contributed by [sebastianmattar](https://github.com/sebastianmattar)
- [#435](https://github.com/WireMock-Net/WireMock.Net/pull/435) - Also call HandlebarsRegistrationCallback when using WithCallback(..) [feature] contributed by [StefH](https://github.com/StefH)
- [#408](https://github.com/WireMock-Net/WireMock.Net/issues/408) - Intermittent threading errors with FindLogEntries [bug]
- [#433](https://github.com/WireMock-Net/WireMock.Net/issues/433) - HandlebarsRegistrationCallback not fired [feature]

# 1.1.9.0 (25 February 2020)
- [#431](https://github.com/WireMock-Net/WireMock.Net/pull/431) - Fix LinqMatcher for JSON int64 [bug] contributed by [StefH](https://github.com/StefH)
- [#425](https://github.com/WireMock-Net/WireMock.Net/issues/425) - Allow 64 bit numbers in JSON [bug]

# 1.1.8.0 (22 February 2020)
- [#419](https://github.com/WireMock-Net/WireMock.Net/pull/419) - Support multi line wild card matching [bug] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#421](https://github.com/WireMock-Net/WireMock.Net/pull/421) - Fix: do not return empty matchers array when Func has been used [bug] contributed by [StefH](https://github.com/StefH)
- [#423](https://github.com/WireMock-Net/WireMock.Net/pull/423) - Fixes for Cookie and Header Reject on Match [bug] contributed by [StefH](https://github.com/StefH)
- [#424](https://github.com/WireMock-Net/WireMock.Net/pull/424) - Don't return empty dictionary object for response headers in JSON mapping [feature] contributed by [StefH](https://github.com/StefH)
- [#418](https://github.com/WireMock-Net/WireMock.Net/issues/418) - Body matching fails if body has newline [bug]

# 1.1.7.0 (06 February 2020)
- [#409](https://github.com/WireMock-Net/WireMock.Net/pull/409) - Admin Delete with mappings in body [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#411](https://github.com/WireMock-Net/WireMock.Net/pull/411) - Improved relative path checking based on file existence [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#413](https://github.com/WireMock-Net/WireMock.Net/pull/413) - Fix new Delete with body missing from IWireMockAdminApi interface contributed by [NoahLerner](https://github.com/NoahLerner)
- [#414](https://github.com/WireMock-Net/WireMock.Net/pull/414) - Fix logger in StandAlone [bug] contributed by [StefH](https://github.com/StefH)
- [#412](https://github.com/WireMock-Net/WireMock.Net/issues/412) - WireMock Standalone - null reference exception since settings.Logger [bug]

# 1.1.6.0 (27 January 2020)
- [#407](https://github.com/WireMock-Net/WireMock.Net/pull/407) - AllowAnyHttpStatusCodeInResponse [feature] contributed by [StefH](https://github.com/StefH)

# 1.1.5.0 (25 January 2020)
- [#405](https://github.com/WireMock-Net/WireMock.Net/pull/405) - Fix logging an Exception Message (linux docker on azure) [bug] contributed by [StefH](https://github.com/StefH)
- [#406](https://github.com/WireMock-Net/WireMock.Net/pull/406) - Fixed StatusCode = null or &lt; 0 [bug] contributed by [StefH](https://github.com/StefH)

# 1.1.3.0 (22 January 2020)
- [#403](https://github.com/WireMock-Net/WireMock.Net/pull/403) - Fix for invalid cast exception contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#402](https://github.com/WireMock-Net/WireMock.Net/issues/402) - Invalid Cast Exception  [bug]

# 1.1.2.0 (09 January 2020)
- [#399](https://github.com/WireMock-Net/WireMock.Net/pull/399) - ResponseModel.StatusCode is deserialized as either string or long. [bug] contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#400](https://github.com/WireMock-Net/WireMock.Net/issues/400) - StatusCode not built correctly when loaded from mapping file. [bug]

# 1.1.1.0 (09 January 2020)
- [#398](https://github.com/WireMock-Net/WireMock.Net/pull/398) - Feature/xpath transformer [feature] contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#397](https://github.com/WireMock-Net/WireMock.Net/issues/397) - Question/Feature: Add support for selecting XPath in response template [feature]

# 1.1.0.0 (27 December 2019)
- [#363](https://github.com/WireMock-Net/WireMock.Net/pull/363) - WireMock.Net version 1.1.x contributed by [StefH](https://github.com/StefH)

# 1.0.43.0 (26 December 2019)
- [#385](https://github.com/WireMock-Net/WireMock.Net/pull/385) - StatusCode as string [feature] contributed by [StefH](https://github.com/StefH)
- [#380](https://github.com/WireMock-Net/WireMock.Net/issues/380) - StatusCode is defined as integer (string is not possible) [bug]
- [#382](https://github.com/WireMock-Net/WireMock.Net/issues/382) - Return same request body [feature]

# 1.0.42.0 (15 December 2019)
- [#391](https://github.com/WireMock-Net/WireMock.Net/pull/391) - Correctly support DateTime pattern as string in ExactMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#383](https://github.com/WireMock-Net/WireMock.Net/issues/383) - ExactMatcher does not accept ISO8601 DateTime? [bug]

# 1.0.41.0 (14 December 2019)
- [#392](https://github.com/WireMock-Net/WireMock.Net/pull/392) - Fix array in JsonMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#390](https://github.com/WireMock-Net/WireMock.Net/issues/390) - JsonMatcher does not match a body containing an array of strings [bug]

# 1.0.40.0 (09 December 2019)
- [#389](https://github.com/WireMock-Net/WireMock.Net/pull/389) - Fix QueryStringParser [bug] contributed by [StefH](https://github.com/StefH)
- [#387](https://github.com/WireMock-Net/WireMock.Net/issues/387) - Query string parameter value which contains %26 does not work with ExactMatcher [bug]

# 1.0.39.0 (07 December 2019)
- [#370](https://github.com/WireMock-Net/WireMock.Net/pull/370) - Add WebProxySettings (use when proxying requests) [feature] contributed by [StefH](https://github.com/StefH)
- [#388](https://github.com/WireMock-Net/WireMock.Net/pull/388) - Transform body as file [bug] contributed by [StefH](https://github.com/StefH)
- [#369](https://github.com/WireMock-Net/WireMock.Net/issues/369) - Question: Is there a way to provide a corporate proxy configuration? [feature]
- [#375](https://github.com/WireMock-Net/WireMock.Net/issues/375) - Proxying does not follow redirects : make this configurable [feature]
- [#386](https://github.com/WireMock-Net/WireMock.Net/issues/386) - Is transforming contents of XML file supported.? [bug]

# 1.0.38.0 (30 November 2019)
- [#376](https://github.com/WireMock-Net/WireMock.Net/pull/376) - Support int values for states and scenario naming [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#378](https://github.com/WireMock-Net/WireMock.Net/pull/378) - Set handlebars dependency for .net 4.5.1 to fixed value [bug] contributed by [StefH](https://github.com/StefH)
- [#381](https://github.com/WireMock-Net/WireMock.Net/pull/381) - Use dotnet default development certificate for .NET Core 2.x [feature] contributed by [StefH](https://github.com/StefH)
- [#377](https://github.com/WireMock-Net/WireMock.Net/issues/377) - Unable to build against .NET 4.5.1 because of Handlebars [bug]

# 1.0.37.0 (08 November 2019)
- [#373](https://github.com/WireMock-Net/WireMock.Net/pull/373) - Make Sonar and WhiteSource optional in the Azure pipelines build [feature] contributed by [StefH](https://github.com/StefH)
- [#374](https://github.com/WireMock-Net/WireMock.Net/pull/374) - WatchStaticMappingsInSubdirectories [feature] contributed by [StefH](https://github.com/StefH)
- [#372](https://github.com/WireMock-Net/WireMock.Net/issues/372) - Reset in WireMock admin API not working fine. [feature]

# 1.0.36.0 (26 October 2019)
- [#360](https://github.com/WireMock-Net/WireMock.Net/pull/360) - Add support for Faults [feature] contributed by [StefH](https://github.com/StefH)
- [#343](https://github.com/WireMock-Net/WireMock.Net/issues/343) - Feature: Please provide support for Bad responses. [feature]

# 1.0.35.0 (25 October 2019)
- [#367](https://github.com/WireMock-Net/WireMock.Net/pull/367) - No symbol NuGets [feature] contributed by [StefH](https://github.com/StefH)
- [#368](https://github.com/WireMock-Net/WireMock.Net/pull/368) - Remove Obsolete annotations [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.34.0 (22 October 2019)
- [#354](https://github.com/WireMock-Net/WireMock.Net/pull/354) - AllowBodyForAllHttpMethods [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#365](https://github.com/WireMock-Net/WireMock.Net/pull/365) - Bump Microsoft.AspNetCore.All from 2.0.8 to 2.0.9 in /examples/WireMock.Net.WebApplication [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#366](https://github.com/WireMock-Net/WireMock.Net/pull/366) - Update ObsoleteAnnotations [feature] contributed by [StefH](https://github.com/StefH)
- [#352](https://github.com/WireMock-Net/WireMock.Net/issues/352) - DELETE request drops the body [feature]

# 1.0.33.0 (12 October 2019)
- [#311](https://github.com/WireMock-Net/WireMock.Net/pull/311) - fix jsonpath matcher [bug] contributed by [StefH](https://github.com/StefH)
- [#324](https://github.com/WireMock-Net/WireMock.Net/pull/324) - Add CSharpCodeMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#353](https://github.com/WireMock-Net/WireMock.Net/pull/353) - Fixed failing admin requests when content type includes a charset (based on idea from Paul Roub) [bug] contributed by [StefH](https://github.com/StefH)
- [#355](https://github.com/WireMock-Net/WireMock.Net/pull/355) - Add Try-Catch to the event LogEntriesChanged [feature] contributed by [StefH](https://github.com/StefH)
- [#357](https://github.com/WireMock-Net/WireMock.Net/pull/357) - Add Proxy Setting for: SaveMappingForStatusCodePattern to only save the mapping when the status code matches the pattern [feature] contributed by [StefH](https://github.com/StefH)
- [#358](https://github.com/WireMock-Net/WireMock.Net/pull/358) - Fix JsonMatcher (parsing DateTimeOffset) contributed by [StefH](https://github.com/StefH)
- [#306](https://github.com/WireMock-Net/WireMock.Net/issues/306) - Writing to the response body is invalid for responses with status code 204 [bug]
- [#307](https://github.com/WireMock-Net/WireMock.Net/issues/307) - JsonPathMatcher always convert to JArray before matching [bug]
- [#329](https://github.com/WireMock-Net/WireMock.Net/issues/329) - Feature: Add support for CSharpCodeMatcher [feature]
- [#350](https://github.com/WireMock-Net/WireMock.Net/issues/350) - Admin requests fail when content type includes a charset [bug]
- [#356](https://github.com/WireMock-Net/WireMock.Net/issues/356) - JsonMatcher not working when JSON contains a DateTimeOffset [bug]

# 1.0.32.0 (20 September 2019)
- [#348](https://github.com/WireMock-Net/WireMock.Net/pull/348) - When posting new mapping, use DateParseHandling.None [bug] contributed by [StefH](https://github.com/StefH)
- [#347](https://github.com/WireMock-Net/WireMock.Net/issues/347) - Query string match on DateTimeOffset is not working [bug]

# 1.0.31.0 (19 September 2019)
- [#334](https://github.com/WireMock-Net/WireMock.Net/pull/334) - Fix issues with Proxy mode and Binary Request Bodies [bug] contributed by [andi0b](https://github.com/andi0b)
- [#338](https://github.com/WireMock-Net/WireMock.Net/pull/338) - Fix ContentType with parameters in Proxy Mode [bug] contributed by [StefH](https://github.com/StefH)
- [#339](https://github.com/WireMock-Net/WireMock.Net/pull/339) - Fix ConcurrentObservableCollection [bug] contributed by [StefH](https://github.com/StefH)
- [#345](https://github.com/WireMock-Net/WireMock.Net/pull/345) - Fix CompareTo in RequestMatchResult [bug] contributed by [StefH](https://github.com/StefH)
- [#346](https://github.com/WireMock-Net/WireMock.Net/pull/346) - Fix recorded requests skipped by request logger contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#337](https://github.com/WireMock-Net/WireMock.Net/issues/337) - Proxy Missing header Content-Type - tried with Recording [bug]
- [#344](https://github.com/WireMock-Net/WireMock.Net/issues/344) - Mapping adding order matters for multiple mappings? [bug]

# 1.0.29.0 (29 August 2019)
- [#328](https://github.com/WireMock-Net/WireMock.Net/pull/328) - Fix LogRequest : Index Out Of Bounds [bug] contributed by [StefH](https://github.com/StefH)
- [#331](https://github.com/WireMock-Net/WireMock.Net/pull/331) - Fix: Collection was modified exception [bug] contributed by [theramis](https://github.com/theramis)
- [#333](https://github.com/WireMock-Net/WireMock.Net/pull/333) - JsonMatcher support IgnoreCase [feature] contributed by [StefH](https://github.com/StefH)
- [#332](https://github.com/WireMock-Net/WireMock.Net/issues/332) - Case sensitive true is ignored for JsonMatcher [feature]

# 1.0.28.0 (20 August 2019)
- [#309](https://github.com/WireMock-Net/WireMock.Net/pull/309) - Fix LogEntries: collection was modified exception [bug] contributed by [StefH](https://github.com/StefH)
- [#314](https://github.com/WireMock-Net/WireMock.Net/pull/314) - RequestLogExpirationDuration : use DateTime.UtcNow [bug] contributed by [StefH](https://github.com/StefH)
- [#316](https://github.com/WireMock-Net/WireMock.Net/pull/316) - Handles case where parameter value contains == [feature] contributed by [lobsteropteryx](https://github.com/lobsteropteryx)
- [#317](https://github.com/WireMock-Net/WireMock.Net/pull/317) - Make SaveMapping and SaveMappingToFile settings independent. [feature] contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#319](https://github.com/WireMock-Net/WireMock.Net/pull/319) - Add blacklist for Request Cookies. contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#322](https://github.com/WireMock-Net/WireMock.Net/pull/322) - Fix MappingMatcher in case of an exception in LinqMatcher. [bug] contributed by [StefH](https://github.com/StefH)
- [#323](https://github.com/WireMock-Net/WireMock.Net/pull/323) - Refactor MappingConverter &amp; MatcherMapper [refactor] contributed by [StefH](https://github.com/StefH)
- [#326](https://github.com/WireMock-Net/WireMock.Net/pull/326) - Fix Parsing Guid in PUT Mapping [bug] contributed by [StefH](https://github.com/StefH)
- [#252](https://github.com/WireMock-Net/WireMock.Net/issues/252) - Proxy with Transform
- [#308](https://github.com/WireMock-Net/WireMock.Net/issues/308) - __admin/requests - &quot;Collection was modified&quot; exception [bug]
- [#313](https://github.com/WireMock-Net/WireMock.Net/issues/313) - RequestLogExpirationDuration - bug [bug]
- [#325](https://github.com/WireMock-Net/WireMock.Net/issues/325) - Admin API: PUT Mapping, FormatException because of wrong parsing of the Query  [bug]

# 1.0.25.0 (23 July 2019)
- [#304](https://github.com/WireMock-Net/WireMock.Net/pull/304) - Support WithBody with multiple matchers [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.24.0 (22 July 2019)
- [#302](https://github.com/WireMock-Net/WireMock.Net/pull/302) - Fixed bug 301 by not setting BodyAsFile to null after first use [bug] contributed by [rwwilden](https://github.com/rwwilden)
- [#301](https://github.com/WireMock-Net/WireMock.Net/issues/301) - Error thrown when calling mocked endpoint second time when using file-based response body [bug]

# 1.0.23.0 (16 July 2019)
- [#298](https://github.com/WireMock-Net/WireMock.Net/pull/298) - MappingModels [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.22.0 (15 July 2019)
- [#297](https://github.com/WireMock-Net/WireMock.Net/pull/297) - FixNullRef (#295) contributed by [StefH](https://github.com/StefH)
- [#295](https://github.com/WireMock-Net/WireMock.Net/issues/295) - NullRef in 1.0.21 [bug]

# 1.0.21.0 (03 July 2019)
- [#286](https://github.com/WireMock-Net/WireMock.Net/pull/286) - Handlebars Extension [feature] contributed by [StefH](https://github.com/StefH)
- [#293](https://github.com/WireMock-Net/WireMock.Net/pull/293) - workaround for AppContext.BaseDirectory being null on some platforms contributed by [eli-darkly](https://github.com/eli-darkly)
- [#294](https://github.com/WireMock-Net/WireMock.Net/pull/294) - don't strip request body if we don't recognize the request method contributed by [eli-darkly](https://github.com/eli-darkly)
- [#289](https://github.com/WireMock-Net/WireMock.Net/issues/289) - Bug:  When WatchStaticMappings=true throws exceptions on updating the mapping files [bug]
- [#290](https://github.com/WireMock-Net/WireMock.Net/issues/290) - Request body is dropped if verb is REPORT [bug]
- [#292](https://github.com/WireMock-Net/WireMock.Net/issues/292) - Can't start server in Xamarin Android [bug]

# 1.0.20.0 (17 June 2019)
- [#284](https://github.com/WireMock-Net/WireMock.Net/pull/284) - Add SaveToFile in the mapping [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.19.0 (15 June 2019)
- [#283](https://github.com/WireMock-Net/WireMock.Net/issues/283) - Support equal-sign in query [bug]

# 1.0.18.0 (10 June 2019)
- [#282](https://github.com/WireMock-Net/WireMock.Net/pull/282) - WireMock.Net.Standalone : Add --WireMockLogger commandline argument [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.17.0 (05 June 2019)
- [#278](https://github.com/WireMock-Net/WireMock.Net/pull/278) - Add support for HandleBars File (to read a file) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.16.0 (16 May 2019)
- [#274](https://github.com/WireMock-Net/WireMock.Net/pull/274) - Sign Assembly [feature] contributed by [StefH](https://github.com/StefH)
- [#160](https://github.com/WireMock-Net/WireMock.Net/issues/160) - Feature: Sign 'WireMock.Net' [feature]
- [#267](https://github.com/WireMock-Net/WireMock.Net/issues/267) - Assembly does not have strong name

# 1.0.15.0 (04 May 2019)
- [#271](https://github.com/WireMock-Net/WireMock.Net/pull/271) - Support Dynamic response files using Handlebars templating [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#272](https://github.com/WireMock-Net/WireMock.Net/pull/272) - Add unit test for JsonPath and BodyAsFile mapping contributed by [denstorti](https://github.com/denstorti)
- [#273](https://github.com/WireMock-Net/WireMock.Net/pull/273) - Dynamic response handlebars templating (2) [bug, feature] contributed by [StefH](https://github.com/StefH)

# 1.0.14.0 (20 April 2019)
- [#269](https://github.com/WireMock-Net/WireMock.Net/pull/269) - Add JmesPath matcher [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.13.0 (11 April 2019)
- [#266](https://github.com/WireMock-Net/WireMock.Net/pull/266) - [265] Add file upload to allow mocking of file operations contributed by [JackCreativeCrew](https://github.com/JackCreativeCrew)
- [#265](https://github.com/WireMock-Net/WireMock.Net/issues/265) - File Upload [feature]

# 1.0.12.0 (05 April 2019)
- [#264](https://github.com/WireMock-Net/WireMock.Net/pull/264) - Proxy : also save multipart as string in mapping file contributed by [StefH](https://github.com/StefH)
- [#263](https://github.com/WireMock-Net/WireMock.Net/issues/263) - Content-Type multipart/form-data is not serialized in proxy and recording mode [bug]

# 1.0.11.0 (30 March 2019)
- [#261](https://github.com/WireMock-Net/WireMock.Net/pull/261) - Fix BodyAsJson transform bug in ResponseMessageTransformer contributed by [psypilat](https://github.com/psypilat)
- [#262](https://github.com/WireMock-Net/WireMock.Net/pull/262) - Add ProvideResponse_WithJsonBodyAndTransform test contributed by [psypilat](https://github.com/psypilat)

# 1.0.10.0 (27 March 2019)
- [#260](https://github.com/WireMock-Net/WireMock.Net/pull/260) - Fix Response.Delay property serialization [bug] contributed by [StefH](https://github.com/StefH)

# 1.0.9.0 (25 March 2019)
- [#256](https://github.com/WireMock-Net/WireMock.Net/pull/256) - Fixed Multi Param Match logic contributed by [StefH](https://github.com/StefH)
- [#255](https://github.com/WireMock-Net/WireMock.Net/issues/255) - ExactMatcher with array pattern not working? [bug]

# 1.0.8.0 (12 March 2019)
- [#254](https://github.com/WireMock-Net/WireMock.Net/pull/254) - RequestMessageParamMatcher supports Ignore Case for the key [feature] contributed by [StefH](https://github.com/StefH)
- [#253](https://github.com/WireMock-Net/WireMock.Net/issues/253) - Request Path and query parameter keys are case-sensitive

# 1.0.7.0 (19 January 2019)
- [#244](https://github.com/WireMock-Net/WireMock.Net/pull/244) - Fix BodyAsFile to also allow relative paths [feature] contributed by [StefH](https://github.com/StefH)
- [#240](https://github.com/WireMock-Net/WireMock.Net/issues/240) - How to submit mappings for multiple request, responses [feature]
- [#243](https://github.com/WireMock-Net/WireMock.Net/issues/243) - Not able to read response from file [bug]

# 1.0.6.1 (10 January 2019)
- [#247](https://github.com/WireMock-Net/WireMock.Net/pull/247) - Issue 225 improve logging in example for wire mock as windows service contributed by [paulssn](https://github.com/paulssn)
- [#249](https://github.com/WireMock-Net/WireMock.Net/pull/249) - Fixed &quot;Content-Type multipart/form-data&quot; [bug] contributed by [StefH](https://github.com/StefH)
- [#225](https://github.com/WireMock-Net/WireMock.Net/issues/225) - Feature: Improve logging in example for WireMock as Windows Service [feature]
- [#248](https://github.com/WireMock-Net/WireMock.Net/issues/248) - Content-Type multipart/form-data is not seen as byte[] anymore

# 1.0.6 (15 December 2018)
- [#242](https://github.com/WireMock-Net/WireMock.Net/pull/242) - Post multiple Mappings [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.5 (07 December 2018)
- [#236](https://github.com/WireMock-Net/WireMock.Net/pull/236) - Add Random Regex (using Fare) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.4.21 (30 November 2018)
- [#221](https://github.com/WireMock-Net/WireMock.Net/pull/221) - Update dependencies [feature] contributed by [StefH](https://github.com/StefH)
- [#229](https://github.com/WireMock-Net/WireMock.Net/pull/229) - Fix proxy tests [test] contributed by [StefH](https://github.com/StefH)
- [#230](https://github.com/WireMock-Net/WireMock.Net/pull/230) - Add HandleBars Random functionality (#219) [feature] contributed by [StefH](https://github.com/StefH)
- [#231](https://github.com/WireMock-Net/WireMock.Net/pull/231) - Use RandomDataGenerator.Net 1.0.3.0 contributed by [StefH](https://github.com/StefH)
- [#232](https://github.com/WireMock-Net/WireMock.Net/pull/232) - Add SonarLint checks [feature] contributed by [StefH](https://github.com/StefH)
- [#233](https://github.com/WireMock-Net/WireMock.Net/pull/233) - RandomDataGenerator.Net 1.0.4 [feature] contributed by [StefH](https://github.com/StefH)
- [#235](https://github.com/WireMock-Net/WireMock.Net/pull/235) - Check aggregate exception during startup [bug] contributed by [StefH](https://github.com/StefH)
- [#219](https://github.com/WireMock-Net/WireMock.Net/issues/219) - Feature: random value helper [feature]
- [#234](https://github.com/WireMock-Net/WireMock.Net/issues/234) - Timeout Exception on VSTS Test Platform (Azure DevOps), with private build agent

# 1.0.4.20 (07 November 2018)
- [#222](https://github.com/WireMock-Net/WireMock.Net/pull/222) - Codecov contributed by [StefH](https://github.com/StefH)
- [#224](https://github.com/WireMock-Net/WireMock.Net/pull/224) - Fixed issue 223: Example for WireMock as Windows Service throws Exception because of WireMockConsoleLogger contributed by [paulssn](https://github.com/paulssn)
- [#228](https://github.com/WireMock-Net/WireMock.Net/pull/228) - Fixed logic for IsRestrictedResponseHeader [bug] contributed by [StefH](https://github.com/StefH)
- [#223](https://github.com/WireMock-Net/WireMock.Net/issues/223) - Bug: Example for WireMock as Windows Service throws Exception because of WireMockConsoleLogger [bug]

# 1.0.4.19 (31 October 2018)
- [#220](https://github.com/WireMock-Net/WireMock.Net/pull/220) - Update SimpleCommandLineParser to handle arguments with key and value contributed by [StefH](https://github.com/StefH)

# 1.0.4.18 (27 October 2018)
- [#207](https://github.com/WireMock-Net/WireMock.Net/pull/207) - Rewrite some unit-integration-tests to unit-tests (#206) contributed by [StefH](https://github.com/StefH)
- [#208](https://github.com/WireMock-Net/WireMock.Net/pull/208) - Refactor contributed by [StefH](https://github.com/StefH)
- [#209](https://github.com/WireMock-Net/WireMock.Net/pull/209) - NET Core 2.1 + support for Service Fabric commandline parameters contributed by [StefH](https://github.com/StefH)
- [#212](https://github.com/WireMock-Net/WireMock.Net/pull/212) - Update BodyParser logic contributed by [StefH](https://github.com/StefH)
- [#216](https://github.com/WireMock-Net/WireMock.Net/pull/216) - ResponseBodyData contributed by [StefH](https://github.com/StefH)
- [#217](https://github.com/WireMock-Net/WireMock.Net/pull/217) - Enable Source Link contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#218](https://github.com/WireMock-Net/WireMock.Net/pull/218) - remove appveyor contributed by [StefH](https://github.com/StefH)
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
- [#203](https://github.com/WireMock-Net/WireMock.Net/pull/203) - Set up CI with Azure Pipelines contributed by [azure-pipelines[bot]](https://github.com/apps/azure-pipelines)
- [#204](https://github.com/WireMock-Net/WireMock.Net/pull/204) - Lower priority from Proxy mappings in favor of Admin Mappings [feature] contributed by [StefH](https://github.com/StefH)
- [#200](https://github.com/WireMock-Net/WireMock.Net/issues/200) - Issue: Incorrect port matching
- [#205](https://github.com/WireMock-Net/WireMock.Net/issues/205) - Issue: DELETE method is proxied as lowercase [bug]

# 1.0.4.16 (11 September 2018)
- [#202](https://github.com/WireMock-Net/WireMock.Net/pull/202) - Update handlebars code to support Regex.Match (#201) contributed by [StefH](https://github.com/StefH)
- [#201](https://github.com/WireMock-Net/WireMock.Net/issues/201) - Question : Extracting text from a request.body that is not json

# 1.0.4.15 (04 September 2018)
- [#199](https://github.com/WireMock-Net/WireMock.Net/pull/199) - Fix for .WithBody(Func&lt;RequestMessage, string&gt;...) contributed by [StefH](https://github.com/StefH)
- [#198](https://github.com/WireMock-Net/WireMock.Net/issues/198) - Issue : creating response using .WithBody(Func&lt;RequestMessage, string&gt;...) and .WithStatusCode [bug]

# 1.0.4.14 (02 September 2018)
- [#197](https://github.com/WireMock-Net/WireMock.Net/pull/197) - Set IsStarted = true in a IApplicationLifetime.ApplicationStarted listener [bug] contributed by [davide-romanini](https://github.com/davide-romanini)
- [#196](https://github.com/WireMock-Net/WireMock.Net/issues/196) - Issue: AspNetCoreSelfHost.IsStarted set before the server actually started for real [bug]

# 1.0.4.13 (31 August 2018)
- [#195](https://github.com/WireMock-Net/WireMock.Net/pull/195) - Add LinqMatcher contributed by [StefH](https://github.com/StefH)
- [#192](https://github.com/WireMock-Net/WireMock.Net/issues/192) - Cannot upgrade from 1.0.4.10 to 1.0.4.12 without upgrading to .net core 2.1 [bug]

# 1.0.4.12 (23 August 2018)
- [#190](https://github.com/WireMock-Net/WireMock.Net/pull/190) - Fix ResponseMessageTransformer (#188) contributed by [StefH](https://github.com/StefH)
- [#191](https://github.com/WireMock-Net/WireMock.Net/pull/191) - Fix ignore case logic for header-name and cookie-name  contributed by [StefH](https://github.com/StefH)
- [#188](https://github.com/WireMock-Net/WireMock.Net/issues/188) - Bug: ResponseMessageTransformer : 
- [#189](https://github.com/WireMock-Net/WireMock.Net/issues/189) - Issue: Case of header key/name not ignored in RequestBuilder when ignoreCase == true

# 1.0.4.11 (20 August 2018)
- [#183](https://github.com/WireMock-Net/WireMock.Net/pull/183) - Set Content-Type header for PutMappingAsync in the client contributed by [seanamosw](https://github.com/seanamosw)
- [#185](https://github.com/WireMock-Net/WireMock.Net/pull/185) - Support Microsoft.AspNetCore for net 4.6.1 and up [feature] contributed by [StefH](https://github.com/StefH)
- [#186](https://github.com/WireMock-Net/WireMock.Net/pull/186) - ContentType &quot;application/vnd.api+json&quot; is not recognized as json contributed by [steveland83](https://github.com/steveland83)
- [#182](https://github.com/WireMock-Net/WireMock.Net/issues/182) - Bug: IFluentMockServerAdmin::PutMappingAsync does not set Content-Type
- [#184](https://github.com/WireMock-Net/WireMock.Net/issues/184) - Bug: Fix AppVeyor PR build process
- [#187](https://github.com/WireMock-Net/WireMock.Net/issues/187) - Bug: Admin GetRequestAsync does not populate request body for JsonApi (&quot;application/vnd.api+json&quot;) content

# 1.0.4.10 (14 August 2018)
- [#180](https://github.com/WireMock-Net/WireMock.Net/pull/180) - Add IFileSystemHandler to support Azure for StaticMapping location contributed by [StefH](https://github.com/StefH)
- [#173](https://github.com/WireMock-Net/WireMock.Net/issues/173) - Feature: Mapping files lost when restarting an Azure app service [feature]

# 1.0.4.9 (08 August 2018)
- [#172](https://github.com/WireMock-Net/WireMock.Net/issues/172) - Question: Same/similar fluent interface for in process and admin client API
- [#174](https://github.com/WireMock-Net/WireMock.Net/issues/174) - Bug: JsonMatcher and JsonPathMatcher throws when posting byte[] [bug]
- [#175](https://github.com/WireMock-Net/WireMock.Net/issues/175) - Bug: Don't allow adding a mapping with no URL or PATH [bug]
- [#176](https://github.com/WireMock-Net/WireMock.Net/issues/176) - Question: Saving mapping with relative (not found) file fails
- [#177](https://github.com/WireMock-Net/WireMock.Net/issues/177) - Feature: Skip invalid static mapping files [feature]

# 1.0.4.8 (23 July 2018)
- [#170](https://github.com/WireMock-Net/WireMock.Net/pull/170) - Support json path in the response contributed by [StefH](https://github.com/StefH)
- [#167](https://github.com/WireMock-Net/WireMock.Net/issues/167) - Feature: Support for JsonPath in the response (with HandleBars) [feature]

# 1.0.4.7 (19 July 2018)
- [#169](https://github.com/WireMock-Net/WireMock.Net/pull/169) - Fix for Restricted Response headers [bug] contributed by [StefH](https://github.com/StefH)
- [#148](https://github.com/WireMock-Net/WireMock.Net/issues/148) - Question: proxy passthrough when no match?

# 1.0.4.6 (18 July 2018)
- [#168](https://github.com/WireMock-Net/WireMock.Net/pull/168) - Expose scenario states [feature] contributed by [StefH](https://github.com/StefH)
- [#163](https://github.com/WireMock-Net/WireMock.Net/issues/163) - Feature: Expose scenario states

# 1.0.4.5 (17 July 2018)
- [#164](https://github.com/WireMock-Net/WireMock.Net/pull/164) - Support running WireMock.Net as a sub-app in IIS [feature] contributed by [StefH](https://github.com/StefH)
- [#165](https://github.com/WireMock-Net/WireMock.Net/pull/165) - Add SonarCloud contributed by [StefH](https://github.com/StefH)
- [#166](https://github.com/WireMock-Net/WireMock.Net/pull/166) - Fix Sonar issues contributed by [StefH](https://github.com/StefH)
- [#120](https://github.com/WireMock-Net/WireMock.Net/issues/120) - Question: JsonPathMatcher - not matching? Correct syntax?
- [#123](https://github.com/WireMock-Net/WireMock.Net/issues/123) - Fix for DateTime Header causing null value in ResponseBuilder

# 1.0.4.4 (01 July 2018)
- [#156](https://github.com/WireMock-Net/WireMock.Net/issues/156) - Feature: when adding / updating a mapping : return more details

# 1.0.4.3 (30 June 2018)
- [#159](https://github.com/WireMock-Net/WireMock.Net/issues/159) - Bug: IRequestBuilder.WithParam broken for key-only matching [bug]

# 1.0.4.2 (26 June 2018)
- [#157](https://github.com/WireMock-Net/WireMock.Net/pull/157) - Support for string and object in JsonMatcher. [feature] contributed by [StefH](https://github.com/StefH)
- [#150](https://github.com/WireMock-Net/WireMock.Net/issues/150) - Add support for .NET Core 2.1 (.NET Core 2.0 will reach end of life on september 2018)
- [#154](https://github.com/WireMock-Net/WireMock.Net/issues/154) - Feature: support BodyAsJson for Request in static mapping files. [feature]

# 1.0.4.1 (25 June 2018)
- [#153](https://github.com/WireMock-Net/WireMock.Net/issues/153) - Feature: Add JsonMatcher to support Json mapping

# 1.0.4.0 (23 June 2018)
- [#131](https://github.com/WireMock-Net/WireMock.Net/issues/131) - Bug: CurlException Couldn't connect to Server when running multiple tests
- [#149](https://github.com/WireMock-Net/WireMock.Net/issues/149) - Question: Transformer and Delay in Static Mappings?
- [#151](https://github.com/WireMock-Net/WireMock.Net/issues/151) - Feature: Add logging of incoming request and body for tracability

# 1.0.3.20 (29 May 2018)
- [#147](https://github.com/WireMock-Net/WireMock.Net/pull/147) - Revert PortUtil.cs changes contributed by [StefH](https://github.com/StefH)
- [#129](https://github.com/WireMock-Net/WireMock.Net/issues/129) - Random test failures between WireMock.Net 1.0.3.1 and 1.0.3.2
- [#146](https://github.com/WireMock-Net/WireMock.Net/issues/146) - Hang possibly due to Windows firewall prompt

# 1.0.3.19 (28 May 2018)
- [#144](https://github.com/WireMock-Net/WireMock.Net/pull/144) - Fix ConcurrentDictionary (#129) contributed by [StefH](https://github.com/StefH)
- [#145](https://github.com/WireMock-Net/WireMock.Net/pull/145) - Cancellation token not passed to server instance in .NET Core 2 [bug] contributed by [Bob11327](https://github.com/Bob11327)

# 1.0.3.18 (25 May 2018)
- [#142](https://github.com/WireMock-Net/WireMock.Net/pull/142) - Allow all headers to be set as Response headers contributed by [StefH](https://github.com/StefH)
- [#122](https://github.com/WireMock-Net/WireMock.Net/issues/122) - WireMock.Net not responding in unit tests - same works in console application
- [#126](https://github.com/WireMock-Net/WireMock.Net/issues/126) - Question: UsingHead always returns 0 for Content-Length header even when explicitly specified
- [#132](https://github.com/WireMock-Net/WireMock.Net/issues/132) - LogEntries not being recorded on subsequent tests
- [#136](https://github.com/WireMock-Net/WireMock.Net/issues/136) - Question: Does the WireMock send Content-Length response header
- [#137](https://github.com/WireMock-Net/WireMock.Net/issues/137) - Question: How to specify Transfer-Encoding response header?
- [#139](https://github.com/WireMock-Net/WireMock.Net/issues/139) - Wiki link https://github.com/StefH/WireMock.Net/wiki/Record-(via-proxy)-and-Save is dead

# 1.0.3.17 (16 May 2018)
- [#134](https://github.com/WireMock-Net/WireMock.Net/pull/134) - Stef negate matcher contributed by [alastairtree](https://github.com/alastairtree)
- [#135](https://github.com/WireMock-Net/WireMock.Net/pull/135) - Merge into the stef_negate_matcher branch (solves issue #133) contributed by [StefH](https://github.com/StefH)
- [#138](https://github.com/WireMock-Net/WireMock.Net/pull/138) - Added Negate matcher logic contributed by [StefH](https://github.com/StefH)
- [#130](https://github.com/WireMock-Net/WireMock.Net/issues/130) - ...

# 1.0.3.16 (17 April 2018)
- [#121](https://github.com/WireMock-Net/WireMock.Net/pull/121) - Fix for issue #118 [bug] contributed by [raghavendrabankapur](https://github.com/raghavendrabankapur)
- [#125](https://github.com/WireMock-Net/WireMock.Net/pull/125) - Change listen from loopback to any ip address for dotnetcore2.0 apps contributed by [SubjectiveReality](https://github.com/SubjectiveReality)
- [#118](https://github.com/WireMock-Net/WireMock.Net/issues/118) - Not reading the response from a file when mappings are placed in json file
- [#124](https://github.com/WireMock-Net/WireMock.Net/issues/124) - Issue: Unable to get host to listen on ips other than 127.0.0.1 using StandAloneApp

# 1.0.3.15 (05 April 2018)
- [#117](https://github.com/WireMock-Net/WireMock.Net/pull/117) - Respect start timeout setting and expose exception from server startup contributed by [evanlwj](https://github.com/evanlwj)

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
- [#95](https://github.com/WireMock-Net/WireMock.Net/pull/95) - Unittest fix contributed by [StefH](https://github.com/StefH)
- [#96](https://github.com/WireMock-Net/WireMock.Net/pull/96) - Replace log4net by custom logger (#94) contributed by [StefH](https://github.com/StefH)
- [#101](https://github.com/WireMock-Net/WireMock.Net/pull/101) - ICallbackResponseBuilder + added more unit-tests [bug] contributed by [StefH](https://github.com/StefH)
- [#102](https://github.com/WireMock-Net/WireMock.Net/pull/102) - Feature: add WithBody(req =&gt; dostuff) style callback [feature] contributed by [alastairtree](https://github.com/alastairtree)
- [#66](https://github.com/WireMock-Net/WireMock.Net/issues/66) - Interested in callbacks?
- [#93](https://github.com/WireMock-Net/WireMock.Net/issues/93) - Bug: FluentMockServer IsStarted after calling Start()
- [#94](https://github.com/WireMock-Net/WireMock.Net/issues/94) - Issue: Introduced dependency on log4net
- [#98](https://github.com/WireMock-Net/WireMock.Net/issues/98) - IBodyResponseBuilder.WithBody* should receive the request as a parameter

# 1.0.3.3 (24 February 2018)
- [#92](https://github.com/WireMock-Net/WireMock.Net/pull/92) - Json fixes (#91) contributed by [StefH](https://github.com/StefH)
- [#91](https://github.com/WireMock-Net/WireMock.Net/issues/91) - Bug: WireMock.Net is not matching application/json http requests using JSONPathMatcher [bug]

# 1.0.3.2 (14 February 2018)
- [#90](https://github.com/WireMock-Net/WireMock.Net/pull/90) - Concurrent issue (#88) contributed by [StefH](https://github.com/StefH)
- [#88](https://github.com/WireMock-Net/WireMock.Net/issues/88) - Bug: Standalone server throws 500 error when receiving concurrent requests [bug]

# 1.0.3.1 (14 February 2018)
- [#89](https://github.com/WireMock-Net/WireMock.Net/pull/89) - Add log4net logging contributed by [StefH](https://github.com/StefH)
- [#87](https://github.com/WireMock-Net/WireMock.Net/issues/87) - Feature: Add logging

# 1.0.3.0 (05 February 2018)
- [#80](https://github.com/WireMock-Net/WireMock.Net/issues/80) - Feature: When using proxy, in case Content-Type is JSON, use BodyAsJson in Response
- [#81](https://github.com/WireMock-Net/WireMock.Net/issues/81) - Feature: When using proxy, only BodyAsBytes in case of binary data?
- [#82](https://github.com/WireMock-Net/WireMock.Net/issues/82) - Feature: make it possible to ignore some headers when proxying [feature]
- [#83](https://github.com/WireMock-Net/WireMock.Net/issues/83) - Feature : Add also a method in IProxyResponseBuilder to provide proxy-settings [feature]
- [#85](https://github.com/WireMock-Net/WireMock.Net/issues/85) - Bug: https for netstandard does not work ? [bug]
- [#86](https://github.com/WireMock-Net/WireMock.Net/issues/86) - Feature : Add FileSystemWatcher logic for watching static mapping files [feature]

# 1.0.2.13 (23 January 2018)
- [#79](https://github.com/WireMock-Net/WireMock.Net/pull/79) - Fix missed content headers contributed by [vladimir-fed](https://github.com/vladimir-fed)
- [#57](https://github.com/WireMock-Net/WireMock.Net/issues/57) - ProxyAndRecord does not save query-parameters, headers and body [bug]
- [#78](https://github.com/WireMock-Net/WireMock.Net/issues/78) - WireMock not working when attempting to access from anything other than localhost.

# 1.0.2.12 (16 January 2018)
- [#75](https://github.com/WireMock-Net/WireMock.Net/pull/75) - Add WireMock.Net.WebApplication example contributed by [StefH](https://github.com/StefH)
- [#77](https://github.com/WireMock-Net/WireMock.Net/pull/77) - Fixed issue #76 contributed by [StefH](https://github.com/StefH)
- [#73](https://github.com/WireMock-Net/WireMock.Net/issues/73) - Updated mapping is not being picked and responded with the response
- [#76](https://github.com/WireMock-Net/WireMock.Net/issues/76) - Bug: IFluentMockServerAdmin is missing content-type for some POST/PUT calls

# 1.0.2.11 (20 December 2017)
- [#72](https://github.com/WireMock-Net/WireMock.Net/issues/72) - Matching WithParam on OData End Points

# 1.0.2.10 (12 December 2017)
- [#70](https://github.com/WireMock-Net/WireMock.Net/issues/70) - Proxy/Intercept pattern is throwing a keep alive header error with net461

# 1.0.2.9 (07 December 2017)
- [#71](https://github.com/WireMock-Net/WireMock.Net/pull/71) - Fixed restricted headers on response contributed by [StefH](https://github.com/StefH)
- [#69](https://github.com/WireMock-Net/WireMock.Net/issues/69) - Instructions are incorrect (?)

# 1.0.2.8 (23 November 2017)
- [#65](https://github.com/WireMock-Net/WireMock.Net/pull/65) - bug: Fix admin api client definition returning the wrong types contributed by [alastairtree](https://github.com/alastairtree)
- [#67](https://github.com/WireMock-Net/WireMock.Net/pull/67) - bug: fix supporting the Patch method and logging the body contributed by [alastairtree](https://github.com/alastairtree)
- [#64](https://github.com/WireMock-Net/WireMock.Net/issues/64) - Pull Requests do not trigger test + codecoverage ?

# 1.0.2.7 (18 November 2017)
- [#62](https://github.com/WireMock-Net/WireMock.Net/pull/62) - Add the Host, Protocol, Port and Origin to the Request message so they can be used in templating contributed by [alastairtree](https://github.com/alastairtree)
- [#63](https://github.com/WireMock-Net/WireMock.Net/pull/63) - Fix issue with concurrent logging contributed by [vladimir-fed](https://github.com/vladimir-fed)
- [#27](https://github.com/WireMock-Net/WireMock.Net/issues/27) - New feature: Record and Save
- [#42](https://github.com/WireMock-Net/WireMock.Net/issues/42) - Enhancement - Save/load request logs to/from disk [feature]
- [#53](https://github.com/WireMock-Net/WireMock.Net/issues/53) - New feature request: Access to Owin pipeline

# 1.0.2.6 (30 October 2017)
- [#59](https://github.com/WireMock-Net/WireMock.Net/pull/59) - Add ability to provide multiple values for headers in response contributed by [Dreamescaper](https://github.com/Dreamescaper)
- [#60](https://github.com/WireMock-Net/WireMock.Net/pull/60) - Fix proxy headers handling contributed by [Dreamescaper](https://github.com/Dreamescaper)
- [#54](https://github.com/WireMock-Net/WireMock.Net/issues/54) - Proxy for AWS: Error unmarshalling response back from AWS [bug]
- [#56](https://github.com/WireMock-Net/WireMock.Net/issues/56) - WithBodyFromFile Support [feature]
- [#58](https://github.com/WireMock-Net/WireMock.Net/issues/58) - Multiple headers with same name [feature]

# 1.0.2.5 (24 October 2017)
- [#55](https://github.com/WireMock-Net/WireMock.Net/pull/55) - Fix the problem with headers passthrough [bug] contributed by [deeptowncitizen](https://github.com/deeptowncitizen)
- [#44](https://github.com/WireMock-Net/WireMock.Net/issues/44) - Bug: Server not listening after Start() returns (on macOS) [bug]
- [#48](https://github.com/WireMock-Net/WireMock.Net/issues/48) - Stateful support [feature]
- [#52](https://github.com/WireMock-Net/WireMock.Net/issues/52) - SimMetrics.NET error when trying to install NuGet Package

# 1.0.2.4 (10 October 2017)
- [#32](https://github.com/WireMock-Net/WireMock.Net/pull/32) - [Feature] Add support for client certificate password and test with real services that require client certificate auth [feature] contributed by [phillee007](https://github.com/phillee007)
- [#35](https://github.com/WireMock-Net/WireMock.Net/pull/35) - Revert changes that were made by mistake in prior PR contributed by [phillee007](https://github.com/phillee007)
- [#39](https://github.com/WireMock-Net/WireMock.Net/pull/39) - Listen on http://*:9090 contributed by [StefH](https://github.com/StefH)
- [#40](https://github.com/WireMock-Net/WireMock.Net/pull/40) - Expose more settings to stand-alone app contributed by [StefH](https://github.com/StefH)
- [#41](https://github.com/WireMock-Net/WireMock.Net/pull/41) - Dotnet 20 preview final [feature] contributed by [StefH](https://github.com/StefH)
- [#45](https://github.com/WireMock-Net/WireMock.Net/pull/45) - Add RequestLogExpirationDuration and MaxRequestLogCount (#43) contributed by [StefH](https://github.com/StefH)
- [#51](https://github.com/WireMock-Net/WireMock.Net/pull/51) - Observable logs contributed by [deeptowncitizen](https://github.com/deeptowncitizen)
- [#15](https://github.com/WireMock-Net/WireMock.Net/issues/15) - New feature: Proxying [feature]
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
- [#26](https://github.com/WireMock-Net/WireMock.Net/pull/26) - merge netstandard into main contributed by [StefH](https://github.com/StefH)
- [#21](https://github.com/WireMock-Net/WireMock.Net/issues/21) - Admin static json mappings [feature]
- [#23](https://github.com/WireMock-Net/WireMock.Net/issues/23) - Consider port to .Net Core
- [#25](https://github.com/WireMock-Net/WireMock.Net/issues/25) - Upgrade to vs2017 [feature]

# 1.0.1.2 (27 February 2017)
- [#24](https://github.com/WireMock-Net/WireMock.Net/pull/24) - Body Encoding contributed by [sbebrys](https://github.com/sbebrys)
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

