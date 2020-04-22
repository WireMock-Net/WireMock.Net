using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using WireMock.Handlers;

namespace WireMock.Transformers
{
    internal static class WireMockHandlebarsHelpers
    {
        public static void Register(IHandlebars handlebarsContext, IFileSystemHandler fileSystemHandler)
        {
            // Register https://github.com/StefH/Handlebars.Net.Helpers
            HandlebarsHelpers.Register(handlebarsContext);

            // Register WireMock.Net specific helpers
            HandlebarsRegex.Register(handlebarsContext);

            HandlebarsJsonPath.Register(handlebarsContext);

            HandlebarsLinq.Register(handlebarsContext);

            HandlebarsRandom.Register(handlebarsContext);

            HandlebarsXeger.Register(handlebarsContext);

            HandlebarsXPath.Register(handlebarsContext);

            HandlebarsFile.Register(handlebarsContext, fileSystemHandler);
        }
    }
}