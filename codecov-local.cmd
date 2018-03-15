rem https://www.appveyor.com/blog/2017/03/17/codecov/

dotnet build .\test\WireMock.Net.Tests\WireMock.Net.Tests.csproj -c Debug

%USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:dotnet.exe -targetargs:"test test\WireMock.Net.Tests\WireMock.Net.Tests.csproj --no-build" -filter:"+[WireMock.Net]* -[WireMock.Net.Tests*]*" -output:coverage.xml -register:user -oldStyle -searchdirs:"test\WireMock.Net.Tests\bin\debug\net452"

%USERPROFILE%\.nuget\packages\ReportGenerator\2.5.6\tools\ReportGenerator.exe -reports:"coverage.xml" -targetdir:"report"

start report\index.htm