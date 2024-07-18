// Copyright © WireMock.Net

using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using WireMock.Net.ConsoleApplication;

namespace WireMock.Net.Console.NETCoreApp;

static class Program
{
    private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

    static void Main(params string[] args)
    {
        XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

        MainApp.Run();
    }
}