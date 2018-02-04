using System;

// copied from https://github.com/aspnet/EntityFramework/blob/dev/src/Microsoft.EntityFrameworkCore/Properties/CoreStrings.resx
namespace WireMock.Validation
{
    // [ExcludeFromCodeCoverage]
    internal static class CoreStrings
    {
        /// <summary>
        /// The property '{property}' of the argument '{argument}' cannot be null.
        /// </summary>
        public static string ArgumentPropertyNull(string property, string argument)
        {
            return $"The property '{property}' of the argument '{argument}' cannot be null.";
        }

        /// <summary>
        /// The string argument '{argumentName}' cannot be empty.
        /// </summary>
        public static string ArgumentIsEmpty(string argumentName)
        {
            return $"The string argument '{argumentName}' cannot be empty.";
        }

        /// <summary>
        /// The entity type '{type}' provided for the argument '{argumentName}' must be a reference type.
        /// </summary>
        public static string InvalidEntityType(Type type, string argumentName)
        {
            return $"The entity type '{type}' provided for the argument '{argumentName}' must be a reference type.";
        }

        /// <summary>
        /// The collection argument '{argumentName}' must contain at least one element.
        /// </summary>
        public static string CollectionArgumentIsEmpty(string argumentName)
        {
            return $"The collection argument '{argumentName}' must contain at least one element.";
        }
    }
}