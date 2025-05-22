// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Helpers;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Transformers.Handlebars;

internal static class WireMockHandlebarsHelpers
{
    internal static void Register(IHandlebars handlebarsContext, WireMockServerSettings settings)
    {
        // Register https://github.com/Handlebars.Net/Handlebars.Net.Helpers
        HandlebarsHelpers.Register(handlebarsContext, o =>
        {
            var paths = new List<string>
            {
                Directory.GetCurrentDirectory(),
                GetBaseDirectory(),
            };

#if !NETSTANDARD1_3_OR_GREATER
            void Add(string? path, ICollection<string> customHelperPaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    customHelperPaths.Add(path!);
                }
            }
            Add(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location), paths);
            Add(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), paths);
            Add(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), paths);
            Add(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName), paths);
#endif
            o.CustomHelperPaths = paths;

            o.CustomHelpers = new Dictionary<string, IHelpers>();
            if (settings.HandlebarsSettings?.AllowedCustomHandlebarsHelpers.HasFlag(CustomHandlebarsHelpers.File) == true)
            {
                o.CustomHelpers.Add(FileHelpers.Name, new FileHelpers(handlebarsContext, settings));
            }
        });
    }

    private static string GetBaseDirectory()
    {
#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER
        return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
#else
        return AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
#endif
    }
}