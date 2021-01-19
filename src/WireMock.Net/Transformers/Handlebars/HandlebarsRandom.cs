using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Validation;

namespace WireMock.Transformers.Handlebars
{
    internal static class HandlebarsRandom
    {
        public static void Register(IHandlebars handlebarsContext)
        {
            handlebarsContext.RegisterHelper("Random", (writer, context, arguments) =>
            {
                object value = GetRandomValue(arguments);
                writer.Write(value);
            });

            handlebarsContext.RegisterHelper("Random", (writer, options, context, arguments) =>
            {
                object value = GetRandomValue(arguments);
                options.Template(writer, value);
            });
        }

        private static object GetRandomValue(object[] arguments)
        {
            var fieldOptions = GetFieldOptionsFromArguments(arguments);
            dynamic randomizer = RandomizerFactory.GetRandomizerAsDynamic(fieldOptions);

            // Format DateTime as ISO 8601
            if (fieldOptions is IFieldOptionsDateTime)
            {
                DateTime? date = randomizer.Generate();
                return date?.ToString("s", CultureInfo.InvariantCulture);
            }

            // If the IFieldOptionsGuid defines Uppercase, use the 'GenerateAsString' method.
            if (fieldOptions is IFieldOptionsGuid fieldOptionsGuid)
            {
                return fieldOptionsGuid.Uppercase ? randomizer.GenerateAsString() : randomizer.Generate();
            }

            return randomizer.Generate();
        }

        private static FieldOptionsAbstract GetFieldOptionsFromArguments(object[] arguments)
        {
            Check.Condition(arguments, args => args.Length > 0, nameof(arguments));
            Check.NotNull(arguments[0], "arguments[0]");

            var properties = (Dictionary<string, object>)arguments[0];
            var newProperties = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> property in properties.Where(p => p.Key != "Type"))
            {
                if (property.Value.GetType().Name == "UndefinedBindingResult")
                {
                    if (TryParseSpecialValue(property.Value, out object parsedValue))
                    {
                        newProperties.Add(property.Key, parsedValue);
                    }
                }
                else
                {
                    newProperties.Add(property.Key, property.Value);
                }
            }

            return FieldOptionsFactory.GetFieldOptions((string)properties["Type"], newProperties);
        }

        /// <summary>
        /// In case it's an UndefinedBindingResult, just try to convert the value using Json
        /// This logic adds functionality like parsing an array
        /// </summary>
        /// <param name="value">The property value</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns>true in case parsing is ok, else false</returns>
        private static bool TryParseSpecialValue(object value, out object parsedValue)
        {
            parsedValue = null;
            string propertyValueAsString = value.ToString();

            try
            {
                JToken jToken = JToken.Parse(propertyValueAsString);
                switch (jToken)
                {
                    case JArray jTokenArray:
                        parsedValue = jTokenArray.ToObject<string[]>().ToList(); // Just convert to a String List to enable Random StringList
                        break;

                    default:
                        return jToken.ToObject<dynamic>();
                }

                return true;
            }
            catch (JsonException)
            {
                // Ignore and don't add this value
            }

            return false;
        }
    }
}