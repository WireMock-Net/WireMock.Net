using System;
using Fare;
using HandlebarsDotNet;
using WireMock.Validation;

namespace WireMock.Transformers.Handlebars
{
    internal static class HandlebarsXeger
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            handlebarsContext.RegisterHelper("Xeger", (writer, context, arguments) =>
            {
                string value = ParseArgumentAndGenerate(arguments);
                writer.Write(value);
            });

            handlebarsContext.RegisterHelper("Xeger", (writer, options, context, arguments) =>
            {
                string value = ParseArgumentAndGenerate(arguments);
                options.Template(writer, value);
            });
        }

        private static string ParseArgumentAndGenerate(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 1, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");

            switch (arguments[0])
            {
                case string pattern:
                    return new Xeger(pattern).Generate();
            }

            throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars Xeger.");
        }
    }
}