﻿using System;
using System.Collections.Generic;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandlebarsHelpers
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("JsonPath.SelectToken", (writer, context, arguments) =>
            {
                (JObject valueToProcess, string jsonpath) = Parse(arguments);

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
                (JObject valueToProcess, string jsonpath) = Parse(arguments);

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

        private static (JObject valueToProcess, string jsonpath) Parse(object[] arguments)
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
                    throw new NotSupportedException($"The value '{arguments[0]}' with type {arguments[0].GetType()} cannot be used in Handlebars JsonPath.");
            }

            return (valueToProcess, arguments[1] as string);
        }
    }
}