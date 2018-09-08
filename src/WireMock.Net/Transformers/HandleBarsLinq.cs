using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using WireMock.Util;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandleBarsLinq
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("Linq", (writer, context, arguments) =>
            {
                (JToken valueToProcess, string linqStatement) = ParseArguments(arguments);

                try
                {
                    object result = ExecuteDynamicLinq(valueToProcess, linqStatement);
                    writer.WriteSafeString(result);
                }
                catch (ParseException)
                {
                    // Ignore ParseException and return
                    return;
                }
            });

            Handlebars.RegisterHelper("Linq", (writer, options, context, arguments) =>
            {
                (JToken valueToProcess, string linqStatement) = ParseArguments(arguments);

                try
                {
                    var result = ExecuteDynamicLinq(valueToProcess, linqStatement);
                    options.Template(writer, result);
                }
                catch (ParseException)
                {
                    // Ignore ParseException and return
                    return;
                }
            });
        }

        private static dynamic ExecuteDynamicLinq(JToken value, string linqStatement)
        {
            // Convert a single object to a Queryable JObject-list with 1 entry.
            var queryable1 = new[] { value }.AsQueryable();

            // Generate the DynamicLinq select statement.
            string dynamicSelect = JsonUtils.GenerateDynamicLinqStatement(value);

            // Execute DynamicLinq Select statement.
            var queryable2 = queryable1.Select(dynamicSelect);

            // Execute the Select(...) method and get first result with FirstOrDefault().
            return queryable2.Select(linqStatement).FirstOrDefault();
        }

        private static (JToken valueToProcess, string linqStatement) ParseArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length == 2, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");
            Check.NotNullOrEmpty(arguments[1] as string, "arguments[1]");

            JToken valueToProcess;
            switch (arguments[0])
            {
                case string jsonAsString:
                    valueToProcess = new JValue(jsonAsString);
                    break;

                case JToken jsonAsJObject:
                    valueToProcess = jsonAsJObject;
                    break;

                default:
                    throw new NotSupportedException($"The value '{arguments[0]}' with type '{arguments[0]?.GetType()}' cannot be used in Handlebars Linq.");
            }

            return (valueToProcess, arguments[1] as string);
        }
    }
}