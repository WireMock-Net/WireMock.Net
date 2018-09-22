# Build info
For building and running all code in VSCode:

- download nuget.exe from https://www.nuget.org/downloads
- copy nuget.exe to a folder which is listed in the path or just in c:\Windows
- go to the root from this project and run `nuget restore`
- all packages are now restored into the `WireMock.Net\packages` folder

### Note
An example project like `WireMock.Net.Console.Net452.Classic` still shows some red errors in VSCode, but you can just run `dotnet build`.
But you can just execute `.\bin\Debug\WireMock.Net.ConsoleApplication.exe` to run the application