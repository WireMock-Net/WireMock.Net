using HandlebarsDotNet;
using WireMock.Handlers;

namespace WireMock.Transformers
{
    internal static class HandlebarsHelpers
    {
        public static void Register(IHandlebars handlebarsContext, IFileSystemHandler fileSystemHandler)
        {
            HandleBarsRegex.Register(handlebarsContext);

            HandleBarsJsonPath.Register(handlebarsContext);

            HandleBarsLinq.Register(handlebarsContext);

            HandleBarsRandom.Register(handlebarsContext);

            HandleBarsXeger.Register(handlebarsContext);

            HandleBarsFileFragment.Register(handlebarsContext, fileSystemHandler);
        }
    }
}