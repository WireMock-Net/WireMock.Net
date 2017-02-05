using System;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using WireMock.Server;

namespace WireMock.Net.StandAlone
{
    public class Program
    {
        private class Options
        {
            [ValueArgument(typeof(string), 'u', "Urls", Description = "URL(s) to listen on", Optional = false, AllowMultiple = true)]
            public string[] Urls;

            [SwitchArgument('p', "AllowPartialMapping", true, Description = "Allow Partial Mapping (default set to true)", Optional = true)]
            public bool AllowPartialMapping;
        }

        static void Main(params string[] args)
        {
            var options = new Options();
            var parser = new CommandLineParser.CommandLineParser();
            parser.ExtractArgumentAttributes(options);
            parser.ParseCommandLine(args);

            try
            {
                parser.ParseCommandLine(args);

                var server = FluentMockServer.StartWithAdminInterface(options.Urls);

                if (options.AllowPartialMapping)
                    server.AllowPartialMapping();

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