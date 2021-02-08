using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Helpers;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars
{
    internal static class WireMockHandlebarsHelpers
    {
        public static void Register(IHandlebars handlebarsContext, IFileSystemHandler fileSystemHandler)
        {
            // Register https://github.com/StefH/Handlebars.Net.Helpers
            HandlebarsHelpers.Register(handlebarsContext, o =>
            {
                o.UseCategoryPrefix = false;

                o.CustomHelperPaths = new string[]
                {
                    Directory.GetCurrentDirectory()
#if !NETSTANDARD1_3
                    , Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
#endif
                }
                .Distinct()
                .ToList();

                o.CustomHelpers = new Dictionary<string, IHelpers>
                {
                    { "File", new FileHelpers(handlebarsContext, fileSystemHandler) }
                };
            });
        }
    }
}