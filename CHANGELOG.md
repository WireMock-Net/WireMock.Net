# 1.0.4.16 (11 September 2018)

 - [#202](https://github.com/WireMock-Net/WireMock.Net/pull/202) - Update handlebars code to support Regex.Match (#201) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#201](https://github.com/WireMock-Net/WireMock.Net/issues/201) - Question : Extracting text from a request.body that is not json

Commits: f3c395833a...da64934c13


# 1.0.4.15 (04 September 2018)

 - [#199](https://github.com/WireMock-Net/WireMock.Net/pull/199) - Fix for .WithBody(Func<RequestMessage, string>...) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#198](https://github.com/WireMock-Net/WireMock.Net/issues/198) - Issue : creating response using .WithBody(Func<RequestMessage, string>...) and .WithStatusCode +fix

Commits: 41fd1ef99d...41fd1ef99d


# 1.0.4.14 (02 September 2018)

 - [#197](https://github.com/WireMock-Net/WireMock.Net/pull/197) - Set IsStarted = true in a IApplicationLifetime.ApplicationStarted listener contributed by ([davide-romanini](https://github.com/davide-romanini)) +fix
 - [#196](https://github.com/WireMock-Net/WireMock.Net/issues/196) - Issue: AspNetCoreSelfHost.IsStarted set before the server actually started for real +fix

Commits: 077b3c0891...5ccb992201


# 1.0.4.13 (31 August 2018)

 - [#195](https://github.com/WireMock-Net/WireMock.Net/pull/195) - Add LinqMatcher contributed by Stef Heyenrath ([StefH](https://github.com/StefH))

Commits: 9f17948e9f...9f17948e9f


# 1.0.4.12 (23 August 2018)

 - [#191](https://github.com/WireMock-Net/WireMock.Net/pull/191) - Fix ignore case logic for header-name and cookie-name contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#190](https://github.com/WireMock-Net/WireMock.Net/pull/190) - Fix ResponseMessageTransformer (#188) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#189](https://github.com/WireMock-Net/WireMock.Net/issues/189) - Issue: Case of header key/name not ignored in RequestBuilder when ignoreCase == true
 - [#188](https://github.com/WireMock-Net/WireMock.Net/issues/188) - Bug: ResponseMessageTransformer : 

Commits: e6ecf5cc84...f8d22d4c47


# 1.0.4.11 (20 August 2018)

 - [#187](https://github.com/WireMock-Net/WireMock.Net/issues/187) - Bug: Admin GetRequestAsync does not populate request body for JsonApi ("application/vnd.api+json") content
 - [#186](https://github.com/WireMock-Net/WireMock.Net/pull/186) - ContentType "application/vnd.api+json" is not recognized as json contributed by Steve Land ([steveland83](https://github.com/steveland83))
 - [#185](https://github.com/WireMock-Net/WireMock.Net/pull/185) - Support Microsoft.AspNetCore for net 4.6.1 and up contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +feature
 - [#184](https://github.com/WireMock-Net/WireMock.Net/issues/184) - Bug: Fix AppVeyor PR build process
 - [#183](https://github.com/WireMock-Net/WireMock.Net/pull/183) - Set Content-Type header for PutMappingAsync in the client contributed by ([seanamosw](https://github.com/seanamosw))
 - [#182](https://github.com/WireMock-Net/WireMock.Net/issues/182) - Bug: IFluentMockServerAdmin::PutMappingAsync does not set Content-Type

Commits: 01d6dce62d...be08c3175e


# 1.0.4.10 (14 August 2018)

 - [#180](https://github.com/WireMock-Net/WireMock.Net/pull/180) - Add IFileSystemHandler to support Azure for StaticMapping location contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#173](https://github.com/WireMock-Net/WireMock.Net/issues/173) - Feature: Mapping files lost when restarting an Azure app service +feature

Commits: 4b91c05fe7...4b91c05fe7


# 1.0.4.9 (08 August 2018)

 - [#177](https://github.com/WireMock-Net/WireMock.Net/issues/177) - Feature: Skip invalid static mapping files +feature
 - [#176](https://github.com/WireMock-Net/WireMock.Net/issues/176) - Question: Saving mapping with relative (not found) file fails
 - [#175](https://github.com/WireMock-Net/WireMock.Net/issues/175) - Bug: Don't allow adding a mapping with no URL or PATH +fix
 - [#174](https://github.com/WireMock-Net/WireMock.Net/issues/174) - Bug: JsonMatcher and JsonPathMatcher throws when posting byte[] +fix
 - [#172](https://github.com/WireMock-Net/WireMock.Net/issues/172) - Question: Same/similar fluent interface for in process and admin client API

Commits: 8cb15b2311...d9fde9329a


# 1.0.4.8 (23 July 2018)

 - [#170](https://github.com/WireMock-Net/WireMock.Net/pull/170) - Support json path in the response contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#167](https://github.com/WireMock-Net/WireMock.Net/issues/167) - Feature: Support for JsonPath in the response (with HandleBars) +feature

Commits: 1f226f7361...1f226f7361


# 1.0.4.7 (19 July 2018)

 - [#169](https://github.com/WireMock-Net/WireMock.Net/pull/169) - Fix for Restricted Response headers contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +fix
 - [#148](https://github.com/WireMock-Net/WireMock.Net/issues/148) - Question: proxy passthrough when no match?

Commits: b2bf63b013...b2bf63b013


# 1.0.4.6 (18 July 2018)

 - [#168](https://github.com/WireMock-Net/WireMock.Net/pull/168) - Expose scenario states contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +feature
 - [#163](https://github.com/WireMock-Net/WireMock.Net/issues/163) - Feature: Expose scenario states

Commits: 6b0924029f...8f34291ea9


# 1.0.4.5 (17 July 2018)

 - [#166](https://github.com/WireMock-Net/WireMock.Net/pull/166) - Fix Sonar issues contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#165](https://github.com/WireMock-Net/WireMock.Net/pull/165) - Add SonarCloud contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#164](https://github.com/WireMock-Net/WireMock.Net/pull/164) - Support running WireMock.Net as a sub-app in IIS contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +feature
 - [#158](https://github.com/WireMock-Net/WireMock.Net/issues/158) - Feature: Support running WireMock.Net as a sub-app in IIS
 - [#123](https://github.com/WireMock-Net/WireMock.Net/issues/123) - Fix for DateTime Header causing null value in ResponseBuilder
 - [#120](https://github.com/WireMock-Net/WireMock.Net/issues/120) - Question: JsonPathMatcher - not matching? Correct syntax?
 - [#105](https://github.com/WireMock-Net/WireMock.Net/issues/105) - Question: URL binding issues

Commits: 3125c1bead...a9c0c6b670


# 1.0.4.4 (01 July 2018)

 - [#156](https://github.com/WireMock-Net/WireMock.Net/issues/156) - Feature: when adding / updating a mapping : return more details

Commits: ...


# 1.0.4.3 (30 June 2018)

 - [#159](https://github.com/WireMock-Net/WireMock.Net/issues/159) - Bug: IRequestBuilder.WithParam broken for key-only matching +fix
 - [#156](https://github.com/WireMock-Net/WireMock.Net/issues/156) - Feature: when adding / updating a mapping : return more details

Commits: 2d1ead25cd...db013a56ad


# 1.0.4.2 (26 June 2018)

 - [#157](https://github.com/WireMock-Net/WireMock.Net/pull/157) - Support for string and object in JsonMatcher. contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +feature
 - [#155](https://github.com/WireMock-Net/WireMock.Net/pull/155) - Replace JsonMatcher with JsonObjectMatcher and directly support JSON body matching. contributed by ([DavidKorn](https://github.com/DavidKorn))
 - [#154](https://github.com/WireMock-Net/WireMock.Net/issues/154) - Feature: support BodyAsJson for Request in static mapping files. +feature

Commits: 9470130d65...4283732b6c


# 1.0.4.1 (25 June 2018)

 - [#153](https://github.com/WireMock-Net/WireMock.Net/issues/153) - Feature: Add JsonMatcher to support Json mapping

Commits: f61a814ab5...2eff243a96


# 1.0.4.0 (23 June 2018)

 - [#151](https://github.com/WireMock-Net/WireMock.Net/issues/151) - Feature: Add logging of incoming request and body for tracability
 - [#149](https://github.com/WireMock-Net/WireMock.Net/issues/149) - Question: Transformer and Delay in Static Mappings?
 - [#131](https://github.com/WireMock-Net/WireMock.Net/issues/131) - Bug: CurlException Couldn't connect to Server when running multiple tests

Commits: 443fc76773...443fc76773


# 1.0.3.20 (29 May 2018)

 - [#147](https://github.com/WireMock-Net/WireMock.Net/pull/147) - Revert PortUtil.cs changes contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#146](https://github.com/WireMock-Net/WireMock.Net/issues/146) - Hang possibly due to Windows firewall prompt
 - [#129](https://github.com/WireMock-Net/WireMock.Net/issues/129) - Random test failures between WireMock.Net 1.0.3.1 and 1.0.3.2

Commits: 2fcfda49c7...2b498f45cb


# 1.0.3.19 (28 May 2018)

 - [#129](https://github.com/WireMock-Net/WireMock.Net/issues/129) - Random test failures between WireMock.Net 1.0.3.1 and 1.0.3.2
 - [#145](https://github.com/WireMock-Net/WireMock.Net/pull/145) - Cancellation token not passed to server instance in .NET Core 2 contributed by Bob Paul ([Bob11327](https://github.com/Bob11327)) +fix
 - [#144](https://github.com/WireMock-Net/WireMock.Net/pull/144) - Fix ConcurrentDictionary (#129) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))

Commits: ...


# 1.0.3.18 (25 May 2018)

 - [#142](https://github.com/WireMock-Net/WireMock.Net/pull/142) - Allow all headers to be set as Response headers contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#140](https://github.com/WireMock-Net/WireMock.Net/issues/140) - Question: Why the Microsoft.Owin.Host.HttpListener is not referenced in the dll, which uses WireMock?
 - [#139](https://github.com/WireMock-Net/WireMock.Net/issues/139) - Wiki link https://github.com/StefH/WireMock.Net/wiki/Record-(via-proxy)-and-Save is dead
 - [#137](https://github.com/WireMock-Net/WireMock.Net/issues/137) - Question: How to specify Transfer-Encoding response header?
 - [#136](https://github.com/WireMock-Net/WireMock.Net/issues/136) - Question: Does the WireMock send Content-Length response header
 - [#132](https://github.com/WireMock-Net/WireMock.Net/issues/132) - LogEntries not being recorded on subsequent tests
 - [#127](https://github.com/WireMock-Net/WireMock.Net/issues/127) - Question: Stub priority - Most recent stub is not always used
 - [#126](https://github.com/WireMock-Net/WireMock.Net/issues/126) - Question: UsingHead always returns 0 for Content-Length header even when explicitly specified
 - [#122](https://github.com/WireMock-Net/WireMock.Net/issues/122) - WireMock.Net not responding in unit tests - same works in console application
 - [#97](https://github.com/WireMock-Net/WireMock.Net/issues/97) - Request matching logic is not practical

Commits: eda71bd725...eda71bd725


# 1.0.3.17 (16 May 2018)

 - [#138](https://github.com/WireMock-Net/WireMock.Net/pull/138) - Added Negate matcher logic contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#135](https://github.com/WireMock-Net/WireMock.Net/pull/135) - Merge into the stef_negate_matcher branch (solves issue #133) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#134](https://github.com/WireMock-Net/WireMock.Net/pull/134) - Stef negate matcher contributed by Alastair Crabtree ([alastairtree](https://github.com/alastairtree))
 - [#133](https://github.com/WireMock-Net/WireMock.Net/issues/133) - Issue: Wildcard matching a json body does not work? +fix
 - [#128](https://github.com/WireMock-Net/WireMock.Net/issues/128) - Feature: Negate a matcher +feature
 - [#126](https://github.com/WireMock-Net/WireMock.Net/issues/126) - Question: UsingHead always returns 0 for Content-Length header even when explicitly specified
 - [#103](https://github.com/WireMock-Net/WireMock.Net/issues/103) - Support for Faults

Commits: 0fb4b62b50...c575ca8296


# 1.0.3.16 (15 April 2018)

 - [#125](https://github.com/WireMock-Net/WireMock.Net/pull/125) - Change listen from loopback to any ip address for dotnetcore2.0 apps contributed by ([SubjectiveReality](https://github.com/SubjectiveReality))
 - [#124](https://github.com/WireMock-Net/WireMock.Net/issues/124) - Issue: Unable to get host to listen on ips other than 127.0.0.1 using StandAloneApp
 - [#121](https://github.com/WireMock-Net/WireMock.Net/pull/121) - Fix for issue #118 contributed by ([raghavendrabankapur](https://github.com/raghavendrabankapur)) +fix
 - [#118](https://github.com/WireMock-Net/WireMock.Net/issues/118) - Not reading the response from a file when mappings are placed in json file

Commits: 7bd63a0baf...1bcdfe31ab


# 1.0.3.15 (05 April 2018)

 - [#117](https://github.com/WireMock-Net/WireMock.Net/pull/117) - Respect start timeout setting and expose exception from server startup contributed by Evan Liang ([evanlwj](https://github.com/evanlwj))

Commits: 2d2a2dd6fc...4f294baff2


# 1.0.3.14 (01 April 2018)

 - [#114](https://github.com/WireMock-Net/WireMock.Net/issues/114) - Feature: Add PathSegments in Transform +feature
 - [#113](https://github.com/WireMock-Net/WireMock.Net/issues/113) - Feature: Add BodyAsJsonIndented for response message +feature

Commits: bdb79aec95...e87f970057


# 1.0.3.12 (24 March 2018)

 - [#100](https://github.com/WireMock-Net/WireMock.Net/issues/100) - Issue: JsonPathMatcher - not working for rootless jsons?

Commits: ...


# 1.0.3.11 (20 March 2018)

 - [#110](https://github.com/WireMock-Net/WireMock.Net/issues/110) - Fix: remove `Func[]` from MappingModel

Commits: a4a9f2c862...a4a9f2c862


# 1.0.3.10 (17 March 2018)

 - [#109](https://github.com/WireMock-Net/WireMock.Net/issues/109) - Issue: When proxying, MimeType is wrong for StringContent

Commits: 720c59c595...720c59c595


# 1.0.3.9 (15 March 2018)

 - [#108](https://github.com/WireMock-Net/WireMock.Net/issues/108) - Issue: provide correct contentTypeHeader value for the bodyparser +fix

Commits: f604be3c02...b7dd1b9242


# 1.0.3.8 (10 March 2018)

 - [#106](https://github.com/WireMock-Net/WireMock.Net/issues/106) - Issue: Params does not work, when there are multiple values for a key

Commits: f604be3c02...f604be3c02


# 1.0.3.7 (09 March 2018)

 - [#104](https://github.com/WireMock-Net/WireMock.Net/issues/104) - Issue: PlatformNotSupportedException

Commits: 3e634c2fde...3e634c2fde


# 1.0.3.4 (04 March 2018)

 - [#102](https://github.com/WireMock-Net/WireMock.Net/pull/102) - Feature: add WithBody(req => dostuff) style callback contributed by Alastair Crabtree ([alastairtree](https://github.com/alastairtree)) +feature
 - [#101](https://github.com/WireMock-Net/WireMock.Net/pull/101) - ICallbackResponseBuilder + added more unit-tests contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +fix
 - [#100](https://github.com/WireMock-Net/WireMock.Net/issues/100) - Question: JsonPathMatcher - not working for rootless jsons?
 - [#99](https://github.com/WireMock-Net/WireMock.Net/pull/99) - feat: simple implementation/spike of dynamic responses using callbacks contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#98](https://github.com/WireMock-Net/WireMock.Net/issues/98) - IBodyResponseBuilder.WithBody* should receive the request as a parameter
 - [#96](https://github.com/WireMock-Net/WireMock.Net/pull/96) - Replace log4net by custom logger (#94) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#95](https://github.com/WireMock-Net/WireMock.Net/pull/95) - Unittest fix contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#94](https://github.com/WireMock-Net/WireMock.Net/issues/94) - Issue: Introduced dependency on log4net
 - [#93](https://github.com/WireMock-Net/WireMock.Net/issues/93) - Bug: FluentMockServer IsStarted after calling Start()
 - [#66](https://github.com/WireMock-Net/WireMock.Net/issues/66) - Interested in callbacks?

Commits: e850126184...a2df91a2a2


# 1.0.3.3 (24 February 2018)

 - [#92](https://github.com/WireMock-Net/WireMock.Net/pull/92) - Json fixes (#91) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#91](https://github.com/WireMock-Net/WireMock.Net/issues/91) - Bug: WireMock.Net is not matching application/json http requests using JSONPathMatcher +fix

Commits: 1ffd56701c...ad6c59e3b5


# 1.0.3.2 (14 February 2018)

 - [#90](https://github.com/WireMock-Net/WireMock.Net/pull/90) - Concurrent issue (#88) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#89](https://github.com/WireMock-Net/WireMock.Net/pull/89) - Add log4net logging contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#88](https://github.com/WireMock-Net/WireMock.Net/issues/88) - Bug: Standalone server throws 500 error when receiving concurrent requests +fix
 - [#87](https://github.com/WireMock-Net/WireMock.Net/issues/87) - Feature: Add logging

Commits: 51070dab63...4f87146622


# 1.0.3.0 (05 February 2018)

 - [#86](https://github.com/WireMock-Net/WireMock.Net/issues/86) - Feature : Add FileSystemWatcher logic for watching static mapping files +feature
 - [#85](https://github.com/WireMock-Net/WireMock.Net/issues/85) - Bug: https for netstandard does not work ? +fix
 - [#83](https://github.com/WireMock-Net/WireMock.Net/issues/83) - Feature : Add also a method in IProxyResponseBuilder to provide proxy-settings +feature
 - [#82](https://github.com/WireMock-Net/WireMock.Net/issues/82) - Feature: make it possible to ignore some headers when proxying +feature
 - [#81](https://github.com/WireMock-Net/WireMock.Net/issues/81) - Feature: When using proxy, only BodyAsBytes in case of binary data?
 - [#80](https://github.com/WireMock-Net/WireMock.Net/issues/80) - Feature: When using proxy, in case Content-Type is JSON, use BodyAsJson in Response

Commits: 40ff8514ac...9778c5adbe


# 1.0.2.13 (23 January 2018)

 - [#79](https://github.com/WireMock-Net/WireMock.Net/pull/79) - Fix missed content headers contributed by ([vladimir-fed](https://github.com/vladimir-fed))
 - [#78](https://github.com/WireMock-Net/WireMock.Net/issues/78) - WireMock not working when attempting to access from anything other than localhost.
 - [#57](https://github.com/WireMock-Net/WireMock.Net/issues/57) - ProxyAndRecord does not save query-parameters, headers and body +fix

Commits: 6d60b3773a...cdcaaa970a


# 1.0.2.12 (16 January 2018)

 - [#77](https://github.com/WireMock-Net/WireMock.Net/pull/77) - Fixed issue #76 contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#76](https://github.com/WireMock-Net/WireMock.Net/issues/76) - Bug: IFluentMockServerAdmin is missing content-type for some POST/PUT calls
 - [#75](https://github.com/WireMock-Net/WireMock.Net/pull/75) - Add WireMock.Net.WebApplication example contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#74](https://github.com/WireMock-Net/WireMock.Net/pull/74) - Capturing the index of the existing mapping before removing and insert the updated mapping at the same index of the list contributed by ([raghavendrabankapur](https://github.com/raghavendrabankapur))
 - [#73](https://github.com/WireMock-Net/WireMock.Net/issues/73) - Updated mapping is not being picked and responded with the response

Commits: da798a59aa...e6af765777


# 1.0.2.11 (20 December 2017)

 - [#72](https://github.com/WireMock-Net/WireMock.Net/issues/72) - Matching WithParam on OData End Points

Commits: 71196b51c9...71196b51c9


# 1.0.2.9 (07 December 2017)

 - [#71](https://github.com/WireMock-Net/WireMock.Net/pull/71) - Fixed restricted headers on response contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#69](https://github.com/WireMock-Net/WireMock.Net/issues/69) - Instructions are incorrect (?)

Commits: 601af2d6b2...fd5bc203c3


# 1.0.2.10 (12 December 2017)

 - [#70](https://github.com/WireMock-Net/WireMock.Net/issues/70) - Proxy/Intercept pattern is throwing a keep alive header error with net461

Commits: d0fc889f42...d0fc889f42


# 1.0.2.8 (23 November 2017)

 - [#68](https://github.com/WireMock-Net/WireMock.Net/issues/68) - Full path required in Stub
 - [#67](https://github.com/WireMock-Net/WireMock.Net/pull/67) - bug: fix supporting the Patch method and logging the body contributed by Alastair Crabtree ([alastairtree](https://github.com/alastairtree))
 - [#65](https://github.com/WireMock-Net/WireMock.Net/pull/65) - bug: Fix admin api client definition returning the wrong types contributed by Alastair Crabtree ([alastairtree](https://github.com/alastairtree))
 - [#64](https://github.com/WireMock-Net/WireMock.Net/issues/64) - Pull Requests do not trigger test + codecoverage ?

Commits: d0b48e2967...ea16ee866b


# 1.0.2.7 (18 November 2017)

 - [#63](https://github.com/WireMock-Net/WireMock.Net/pull/63) - Fix issue with concurrent logging contributed by ([vladimir-fed](https://github.com/vladimir-fed))
 - [#62](https://github.com/WireMock-Net/WireMock.Net/pull/62) - Add the Host, Protocol, Port and Origin to the Request message so they can be used in templating contributed by Alastair Crabtree ([alastairtree](https://github.com/alastairtree))
 - [#61](https://github.com/WireMock-Net/WireMock.Net/issues/61) - Partial mapping
 - [#53](https://github.com/WireMock-Net/WireMock.Net/issues/53) - New feature request: Access to Owin pipeline
 - [#42](https://github.com/WireMock-Net/WireMock.Net/issues/42) - Enhancement - Save/load request logs to/from disk +feature
 - [#27](https://github.com/WireMock-Net/WireMock.Net/issues/27) - New feature: Record and Save

Commits: e25c873765...018d2a904d


# 1.0.2.6 (30 October 2017)

 - [#60](https://github.com/WireMock-Net/WireMock.Net/pull/60) - Fix proxy headers handling contributed by Oleksandr Liakhevych ([Dreamescaper](https://github.com/Dreamescaper))
 - [#59](https://github.com/WireMock-Net/WireMock.Net/pull/59) - Add ability to provide multiple values for headers in response contributed by Oleksandr Liakhevych ([Dreamescaper](https://github.com/Dreamescaper))
 - [#58](https://github.com/WireMock-Net/WireMock.Net/issues/58) - Multiple headers with same name +feature
 - [#56](https://github.com/WireMock-Net/WireMock.Net/issues/56) - WithBodyFromFile Support +feature
 - [#54](https://github.com/WireMock-Net/WireMock.Net/issues/54) - Proxy for AWS: Error unmarshalling response back from AWS +fix

Commits: cbe6a0a2b4...d83f308591


# 1.0.2.5 (24 October 2017)

 - [#55](https://github.com/WireMock-Net/WireMock.Net/pull/55) - Fix the problem with headers passthrough contributed by deeptowncitizen ([deeptowncitizen](https://github.com/deeptowncitizen)) +fix
 - [#52](https://github.com/WireMock-Net/WireMock.Net/issues/52) - SimMetrics.NET error when trying to install NuGet Package
 - [#48](https://github.com/WireMock-Net/WireMock.Net/issues/48) - Stateful support +feature
 - [#44](https://github.com/WireMock-Net/WireMock.Net/issues/44) - Bug: Server not listening after Start() returns (on macOS) +fix

Commits: 7c289d44a7...15370a89ca


# 1.0.2.4 (10 October 2017)

 - [#51](https://github.com/WireMock-Net/WireMock.Net/pull/51) - Observable logs contributed by deeptowncitizen ([deeptowncitizen](https://github.com/deeptowncitizen))
 - [#50](https://github.com/WireMock-Net/WireMock.Net/issues/50) - New Feature: Callbacks
 - [#49](https://github.com/WireMock-Net/WireMock.Net/pull/49) - stateful behavior contributed by deeptowncitizen ([deeptowncitizen](https://github.com/deeptowncitizen))
 - [#47](https://github.com/WireMock-Net/WireMock.Net/issues/47) - Feature: add matcher details to Request to see which matchers match/not match +feature
 - [#46](https://github.com/WireMock-Net/WireMock.Net/issues/46) - Log the ip-address from the client/caller also in the RequestLog +feature
 - [#45](https://github.com/WireMock-Net/WireMock.Net/pull/45) - Add RequestLogExpirationDuration and MaxRequestLogCount (#43) contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#43](https://github.com/WireMock-Net/WireMock.Net/issues/43) - Feature: Add RequestLogExpirationDuration and MaxRequestLogCount
 - [#41](https://github.com/WireMock-Net/WireMock.Net/pull/41) - Dotnet 20 preview final contributed by Stef Heyenrath ([StefH](https://github.com/StefH)) +feature
 - [#40](https://github.com/WireMock-Net/WireMock.Net/pull/40) - Expose more settings to stand-alone app contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#39](https://github.com/WireMock-Net/WireMock.Net/pull/39) - Listen on http://*:9090 contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#38](https://github.com/WireMock-Net/WireMock.Net/issues/38) - Bug: support also listening on *:{port}
 - [#37](https://github.com/WireMock-Net/WireMock.Net/issues/37) - Wrong Request Match result is returning
 - [#36](https://github.com/WireMock-Net/WireMock.Net/issues/36) - How to implement a request body-dependent response?
 - [#35](https://github.com/WireMock-Net/WireMock.Net/pull/35) - Revert changes that were made by mistake in prior PR contributed by ([phillee007](https://github.com/phillee007))
 - [#34](https://github.com/WireMock-Net/WireMock.Net/issues/34) - Where is SearchLogsFor method?
 - [#33](https://github.com/WireMock-Net/WireMock.Net/issues/33) - Issue with launching sample code (StandAlone server) +fix
 - [#32](https://github.com/WireMock-Net/WireMock.Net/pull/32) - [Feature] Add support for client certificate password and test with real services that require client certificate auth contributed by ([phillee007](https://github.com/phillee007)) +feature
 - [#31](https://github.com/WireMock-Net/WireMock.Net/issues/31) - Feature request: Nuget package for standalone version +feature
 - [#20](https://github.com/WireMock-Net/WireMock.Net/issues/20) - Add client certificate authentication
 - [#19](https://github.com/WireMock-Net/WireMock.Net/issues/19) - Is this the same as Mock4Net?
 - [#15](https://github.com/WireMock-Net/WireMock.Net/issues/15) - New feature: Proxying +feature

Commits: 538195551d...e87e09e10d


# 1.02.1 (14 June 2017)

 - [#30](https://github.com/WireMock-Net/WireMock.Net/issues/30) - [Feature] Disable partial mappings by default in standalone version +fix
 - [#29](https://github.com/WireMock-Net/WireMock.Net/issues/29) - Support of .Net 4.0
 - [#28](https://github.com/WireMock-Net/WireMock.Net/issues/28) - Facing issue with WildcardMatcher and '?'

Commits: 84db9bbf0d...7111ab384b


# 1.0.2.0 (05 May 2017)

 - [#26](https://github.com/WireMock-Net/WireMock.Net/pull/26) - merge netstandard into main contributed by Stef Heyenrath ([StefH](https://github.com/StefH))
 - [#25](https://github.com/WireMock-Net/WireMock.Net/issues/25) - Upgrade to vs2017 +feature
 - [#23](https://github.com/WireMock-Net/WireMock.Net/issues/23) - Consider port to .Net Core
 - [#21](https://github.com/WireMock-Net/WireMock.Net/issues/21) - Admin static json mappings +feature

Commits: b547993415...8d9cef6dd1


# 1.0.1.2 (27 February 2017)

 - [#24](https://github.com/WireMock-Net/WireMock.Net/pull/24) - Body Encoding contributed by Sebastian Bebrys ([sbebrys](https://github.com/sbebrys))
 - [#22](https://github.com/WireMock-Net/WireMock.Net/issues/22) - Add basic-authentication for accessing admin-interface +feature
 - [#8](https://github.com/WireMock-Net/WireMock.Net/issues/8) - admin rest api

Commits: bb35f55bbb...02803562c6
