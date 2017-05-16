using System;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone
{
    public class Program
    {
        private class Options
        {
            [ValueArgument(typeof(string), "Urls", Description = "URL(s) to listen on.", Optional = true, AllowMultiple = true)]
            public List<string> Urls { get; set; }

            [SwitchArgument("AllowPartialMapping", true, Description = "Allow Partial Mapping (default set to true).", Optional = true)]
            public bool AllowPartialMapping { get; set; }

            [SwitchArgument("StartAdminInterface", true, Description = "Start the AdminInterface (default set to true).", Optional = true)]
            public bool StartAdminInterface { get; set; }

            [SwitchArgument("ReadStaticMappings", true, Description = "Read StaticMappings from ./__admin/mappings (default set to true).", Optional = true)]
            public bool ReadStaticMappings { get; set; }

            [ValueArgument(typeof(string), "ProxyURL", Description = "The ProxyURL to use.", Optional = true)]
            public string ProxyURL { get; set; }

            [SwitchArgument("SaveProxyMapping", true, Description = "Save the proxied request and response mapping files in ./__admin/mappings.  (default set to true).", Optional = true)]
            public bool SaveMapping { get; set; }

            [ValueArgument(typeof(string), "X509Certificate2", Description = "The X509Certificate2 Filename to use.", Optional = true)]
            public string X509Certificate2Filename { get; set; }
        }

        static void Main(params string[] args)
        {
            var options = new Options();
            var parser = new CommandLineParser.CommandLineParser();
            parser.ExtractArgumentAttributes(options);

            try
            {
                parser.ParseCommandLine(args);

                if (!options.Urls.Any())
                {
                    options.Urls.Add("http://localhost:9090/");
                }

                var settings = new FluentMockServerSettings
                {
                    Urls = options.Urls.ToArray(),
                    StartAdminInterface = options.StartAdminInterface,
                    ReadStaticMappings = options.ReadStaticMappings,
                };

                if (!string.IsNullOrEmpty(options.ProxyURL))
                {
                    settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                    {
                        Url = options.ProxyURL,
                        SaveMapping = options.SaveMapping,
                        X509Certificate2Filename = options.X509Certificate2Filename
                    };
                }

                var server = FluentMockServer.Start(settings);
                if (options.AllowPartialMapping)
                {
                    server.AllowPartialMapping();
                }

                Console.WriteLine("WireMock.Net server listening at {0}", string.Join(" and ", server.Urls));
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                parser.ShowUsage();
            }

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}