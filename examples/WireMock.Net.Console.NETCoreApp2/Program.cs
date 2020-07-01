using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;
using WireMock.Net.ConsoleApplication;

namespace WireMock.Net.Console.NETCoreApp2
{
    static class Program
    {
        private static readonly ILoggerRepository LogRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main(params string[] args)
        {
            XmlConfigurator.Configure(LogRepository, new FileInfo("log4net.config"));

            foreach (var file in Directory.GetFiles("__admin").Where(f => !f.StartsWith("wiremock")))
            {
                File.Delete(file);
            }

            MainApp.Run();
        }
    }
}