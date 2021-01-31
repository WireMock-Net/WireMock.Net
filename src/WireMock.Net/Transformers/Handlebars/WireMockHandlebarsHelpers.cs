using System.Collections.Generic;
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
                o.CustomHelpers = new Dictionary<string, IHelpers>
                {
                    { "File", new FileHelpers(handlebarsContext, fileSystemHandler) }
                };
            });
        }
    }
}