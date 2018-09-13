dotnet test ..\test\WireMock.Net.Tests\WireMock.Net.Tests.csproj -c Debug -f netcoreapp2.1 /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="../../report/"

%USERPROFILE%\.nuget\packages\ReportGenerator\3.1.2\tools\ReportGenerator.exe -reports:"coverage.opencover.xml" -targetdir:"coverlet"

start coverlet\index.htm