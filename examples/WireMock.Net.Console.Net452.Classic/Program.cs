// Copyright © WireMock.Net

using System.IO;
using log4net.Config;

namespace WireMock.Net.ConsoleApplication;

static class Program
{
    static void Main(params string[] args)
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));

        MainApp.Run();
    }
}