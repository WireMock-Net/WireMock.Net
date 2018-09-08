using System;
using System.Linq;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandleBarsJsonPath
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("JsonPath.SelectToken", (writer, context, arguments) =>
            {
                (JObject valueToProcess, string jsonpath) = ParseArguments(arguments);

                try
                {
                    var result = valueToProcess.SelectToken(jsonpath);
                    writer.WriteSafeString(result);
                }
                catch (JsonException)
                {
                    // Ignore JsonException and return
                    return;
                }
            });

            Handlebars.RegisterHelper("JsonPath.SelectTokens", (writer, options, context, arguments) =>
            {
                (JObject valueToProcess, string jsonpath) = ParseArguments(arguments);

                try
                {
                    var values = valueToProcess.SelectTokens(jsonpath);
                    if (values != null)
                    {
                        options.Template(writer, values.ToDictionary(value => value.Path, value => value));
                    }
                }
                catch (JsonException)
                {
                    // Ignore JsonException and return
                    return;
                }
            });
        }

        private static (JObject valueToProcess, string jsonpath) ParseArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 2, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");
            Check.NotNullOrEmpty(arguments[1] as string, "arguments[1]");

            JObject valueToProcess;

            switch (arguments[0])
            {
                case string jsonAsString:
                    valueToProcess = JObject.Parse(jsonAsString);
                    break;

                case JObject jsonAsJObject:
                    valueToProcess = jsonAsJObject;
                    break;

                default:
                    throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars JsonPath.");
            }

            return (valueToProcess, arguments[1] as string);
        }
    }
}