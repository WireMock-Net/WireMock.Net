using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Utils;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandlebarsHelpers
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("Regex.Match", (writer, context, arguments) =>
            {
                (string stringToProcess, string regexPattern, object defaultValue) = ParseRegexArguments(arguments);

                Match match = Regex.Match(stringToProcess, regexPattern);

                if (match.Success)
                {
                    writer.WriteSafeString(match.Value);
                }
                else if (defaultValue != null)
                {
                    writer.WriteSafeString(defaultValue);
                }
            });

            Handlebars.RegisterHelper("Regex.Match", (writer, options, context, arguments) =>
            {
                (string stringToProcess, string regexPattern, object defaultValue) = ParseRegexArguments(arguments);

                var regex = new Regex(regexPattern);
                var namedGroups = RegexUtils.GetNamedGroups(regex, stringToProcess);
                if (namedGroups.Any())
                {
                    options.Template(writer, namedGroups);
                }
                else if (defaultValue != null)
                {
                    writer.WriteSafeString(defaultValue);
                }
            });

            Handlebars.RegisterHelper("JsonPath.SelectToken", (writer, context, arguments) =>
            {
                (JObject valueToProcess, string jsonpath) = ParseJsonPathArguments(arguments);

                JToken result = null;
                try
                {
                    result = valueToProcess.SelectToken(jsonpath);
                }
                catch (JsonException)
                {
                    // Ignore JsonException and return
                    return;
                }

                if (result != null)
                {
                    writer.WriteSafeString(result);
                }
            });

            Handlebars.RegisterHelper("JsonPath.SelectTokens", (writer, options, context, arguments) =>
            {
                (JObject valueToProcess, string jsonpath) = ParseJsonPathArguments(arguments);

                IEnumerable<JToken> values = null;
                try
                {
                    values = valueToProcess.SelectTokens(jsonpath);
                }
                catch (JsonException)
                {
                    // Ignore JsonException and return
                    return;
                }

                if (values == null)
                {
                    return;
                }

                int id = 0;
                foreach (JToken value in values)
                {
                    options.Template(writer, new { id, value });
                    id++;
                }
            });
        }

        private static (JObject valueToProcess, string jsonpath) ParseJsonPathArguments(object[] arguments)
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

        private static (string stringToProcess, string regexPattern, object defaultValue) ParseRegexArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length >= 2, nameof(arguments));

            string ParseAsString(object arg)
            {
                if (arg is string)
                {
                    return arg as string;
                }
                else
                {
                    throw new NotSupportedException($"The value '{arg}' with type '{arg?.GetType()}' cannot be used in Handlebars Regex.");
                }
            }

            return (ParseAsString(arguments[0]), ParseAsString(arguments[1]), arguments.Length == 3 ? arguments[2] : null);
        }
    }
}