using System;
using System.Linq;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone.Net461
{
    static class Program
    {
        static void Main(string[] args)
        {
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