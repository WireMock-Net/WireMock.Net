// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Helpers;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars;

internal static class WireMockHandlebarsHelpers
{
    public static void Register(IHandlebars handlebarsContext, IFileSystemHandler fileSystemHandler)
    {
        // Register https://github.com/StefH/Handlebars.Net.Helpers
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
            Add(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), paths);
            Add(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), paths);
            Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), paths);
            Add(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName), paths);
#endif
            o.CustomHelperPaths = paths;

            o.CustomHelpers = new Dictionary<string, IHelpers>
            {
                { "File", new FileHelpers(handlebarsContext, fileSystemHandler) }
            };
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