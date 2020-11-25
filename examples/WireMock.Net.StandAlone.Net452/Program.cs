using System;
using System.IO;
using System.Linq;
using log4net.Config;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone.Net452
{
    public class Program
    {
        static void Main(params string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            if (WireMockServerSettingsParser.TryParseArguments(args, out var settings))
            {
                settings.Logger.Debug("WireMock.Net server arguments [{0}]", string.Join(", ", args.Select(a => $"'{a}'")));

                WireMockServer.Start(settings);

                Console.WriteLine("Press any key to stop the server");
                Console.ReadKey();
            }
        }
    }
}