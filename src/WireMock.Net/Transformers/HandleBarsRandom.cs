using System.Collections.Generic;
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
                var fieldOptions = GetFieldOptionsFromArguments(arguments);

                dynamic randomizer = RandomizerFactory.GetRandomizerAsDynamic(fieldOptions);
                if (randomizer.GetType().GetMethod("GenerateAsString") != null)
                {
                    string valueAsString = randomizer.GenerateAsString();
                    writer.WriteSafeString(valueAsString);
                }
                else
                {
                    object valueAsObject = randomizer.Generate();
                    writer.WriteSafeString(valueAsObject);
                }
            });
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