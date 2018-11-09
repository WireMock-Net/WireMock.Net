using System;
using System.Collections.Generic;
using System.Globalization;
using HandlebarsDotNet;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Validation;

namespace WireMock.Transformers
{
    internal static class HandleBarsRandom
    {
        public static void Register()
        {
            Handlebars.RegisterHelper("Random", (writer, context, arguments) =>
            {
                object value = GetValue(arguments);
                writer.Write(value);
            });

            Handlebars.RegisterHelper("Random", (writer, options, context, arguments) =>
            {
                object value = GetValue(arguments);
                options.Template(writer, value);
            });
        }

        private static object GetValue(object[] arguments)
        {
            var fieldOptions = GetFieldOptionsFromArguments(arguments);
            dynamic randomizer = RandomizerFactory.GetRandomizerAsDynamic(fieldOptions);

            // Format DateTime as ISO 8601
            if (fieldOptions is IFieldOptionsDateTime)
            {
                DateTime? date = randomizer.Generate();
                return date.HasValue ? date.Value.ToString("s", CultureInfo.InvariantCulture) : null;
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
            string type = (string)properties["Type"];

            properties.Remove("Type");

            return FieldOptionsFactory.GetFieldOptions(type, properties);
        }
    }
}