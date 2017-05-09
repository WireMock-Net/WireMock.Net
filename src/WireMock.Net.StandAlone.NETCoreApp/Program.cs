using System;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.StandAlone.NETCoreApp
{
    class Program
    {
        private class Options
        {
            [ValueArgument(typeof(string), 'u', "Urls", Description = "URL(s) to listen on.", Optional = true, AllowMultiple = true)]
            public List<string> Urls { get; set; }

            [SwitchArgument('p', "AllowPartialMapping", true, Description = "Allow Partial Mapping (default set to true).", Optional = true)]
            public bool AllowPartialMapping { get; set; }

            [SwitchArgument('s', "StartAdminInterface", true, Description = "Start the AdminInterface (default set to true).", Optional = true)]
            public bool StartAdminInterface { get; set; }

            [SwitchArgument('r', "ReadStaticMappings", true, Description = "Read StaticMappings from ./__admin/mappings (default set to true).", Optional = true)]
            public bool ReadStaticMappings { get; set; }
        }

        static void Main(string[] args)
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

                var server = FluentMockServer.Start(new FluentMockServerSettings
                {
                    Urls = options.Urls.ToArray(),
                    StartAdminInterface = options.StartAdminInterface,
                    ReadStaticMappings = options.ReadStaticMappings
                });

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