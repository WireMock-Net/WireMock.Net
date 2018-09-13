rem TODO
rem https://www.appveyor.com/blog/2017/03/17/codecov/

rem dotnet build .\test\WireMock.Net.Tests\WireMock.Net.Tests.csproj -c Debug

rem USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:dotnet.exe -targetargs:"test test\WireMock.Net.Tests\WireMock.Net.Tests.csproj --no-build" -filter:"+[WireMock.Net]* -[WireMock.Net.Tests*]*" -output:coverage.xml -register:user -oldStyle -searchdirs:"test\WireMock.Net.Tests\bin\debug\net452"

rem %USERPROFILE%\.nuget\packages\ReportGenerator\2.5.6\tools\ReportGenerator.exe -reports:"coverage.xml" -targetdir:"report\opencover"

rem start report\index.htm