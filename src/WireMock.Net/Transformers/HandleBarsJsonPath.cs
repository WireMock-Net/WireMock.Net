using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandlebarsJsonPath
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            handlebarsContext.RegisterHelper("JsonPath.SelectToken", (writer, context, arguments) =>
            {
                (JToken valueToProcess, string jsonPath) = ParseArguments(arguments);

                try
                {
                    var result = valueToProcess.SelectToken(jsonPath);
                    writer.WriteSafeString(result);
                }
                catch (JsonException)
                {
                    // Ignore JsonException
                }
            });

            handlebarsContext.RegisterHelper("JsonPath.SelectTokens", (writer, options, context, arguments) =>
            {
                (JToken valueToProcess, string jsonPath) = ParseArguments(arguments);

                try
                {
                    var values = valueToProcess.SelectTokens(jsonPath);
                    if (values != null)
                    {
                        options.Template(writer, values.ToDictionary(value => value.Path, value => value));
                    }
                }
                catch (JsonException)
                {
                    // Ignore JsonException
                }
            });
        }

        private static (JToken valueToProcess, string jsonpath) ParseArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 2, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");
            Check.NotNullOrEmpty(arguments[1] as string, "arguments[1]");

            JToken valueToProcess;

            switch (arguments[0])
            {
                case JToken tokenValue:
                    valueToProcess = tokenValue;
                    break;

                case string stringValue:
                    valueToProcess = JsonUtils.Parse(stringValue);
                    break;

                case IEnumerable enumerableValue:
                    valueToProcess = JArray.FromObject(enumerableValue);
                    break;

                default:
                    throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars JsonPath.");
            }

            return (valueToProcess, (string)arguments[1]);
        }
    }
}