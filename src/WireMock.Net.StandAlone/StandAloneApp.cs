﻿using System;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Validation;
using JetBrains.Annotations;

namespace WireMock.Net.StandAlone
{
    /// <summary>
    /// The StandAloneApp
    /// </summary>
    public static class StandAloneApp
    {
        private class Options
        {
            [ValueArgument(typeof(string), "Urls", Description = "URL(s) to listen on.", Optional = true, AllowMultiple = true)]
            public List<string> Urls { get; set; }

            [SwitchArgument("AllowPartialMapping", false, Description = "Allow Partial Mapping (default set to false).", Optional = true)]
            public bool AllowPartialMapping { get; set; }

            [SwitchArgument("StartAdminInterface", true, Description = "Start the AdminInterface (default set to true).", Optional = true)]
            public bool StartAdminInterface { get; set; }

            [SwitchArgument("ReadStaticMappings", true, Description = "Read StaticMappings from ./__admin/mappings (default set to true).", Optional = true)]
            public bool ReadStaticMappings { get; set; }

            [ValueArgument(typeof(string), "ProxyURL", Description = "The ProxyURL to use.", Optional = true)]
            public string ProxyURL { get; set; }

            [SwitchArgument("SaveProxyMapping", true, Description = "Save the proxied request and response mapping files in ./__admin/mappings.  (default set to true).", Optional = true)]
            public bool SaveMapping { get; set; }

            [ValueArgument(typeof(string), "X509Certificate2ThumbprintOrSubjectName", Description = "The X509Certificate2 Thumbprint or SubjectName to use.", Optional = true)]
            public string X509Certificate2ThumbprintOrSubjectName { get; set; }

            [ValueArgument(typeof(string), "AdminUsername", Description = "The username needed for __admin access.", Optional = true)]
            public string AdminUsername { get; set; }

            [ValueArgument(typeof(string), "AdminPassword", Description = "The password needed for __admin access.", Optional = true)]
            public string AdminPassword { get; set; }
        }

        /// <summary>
        /// Start WireMock.Net standalone based on the FluentMockServerSettings.
        /// </summary>
        /// <param name="settings">The FluentMockServerSettings</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] FluentMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));

            return FluentMockServer.Start(settings);
        }

        /// <summary>
        /// Start WireMock.Net standalone bases on the commandline arguments.
        /// </summary>
        /// <param name="args">The commandline arguments</param>
        [PublicAPI]
        public static FluentMockServer Start([NotNull] string[] args)
        {
            Check.NotNull(args, nameof(args));

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
                    AllowPartialMapping = options.AllowPartialMapping,
                    AdminUsername = options.AdminUsername,
                    AdminPassword = options.AdminPassword
                };

                if (!string.IsNullOrEmpty(options.ProxyURL))
                {
                    settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                    {
                        Url = options.ProxyURL,
                        SaveMapping = options.SaveMapping,
                        X509Certificate2ThumbprintOrSubjectName = options.X509Certificate2ThumbprintOrSubjectName
                    };
                }

                FluentMockServer server = Start(settings);

                Console.WriteLine("WireMock.Net server listening at {0}", string.Join(" and ", server.Urls));

                return server;
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                parser.ShowUsage();

                throw;
            }
        }
    }
}