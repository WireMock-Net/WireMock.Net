using HandlebarsDotNet;

namespace WireMock.Transformers
{
    internal static class HandlebarsHelpers
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            HandleBarsRegex.Register(handlebarsContext);

            HandleBarsJsonPath.Register(handlebarsContext);

            HandleBarsLinq.Register(handlebarsContext);

            HandleBarsRandom.Register(handlebarsContext);

            HandleBarsXeger.Register(handlebarsContext);
        }
    }
}