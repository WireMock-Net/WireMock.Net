using System;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A interface defining the example values to use for the different types.
    /// </summary>
    public interface IWireMockOpenApiParserExampleValues
    {
        /// <summary>
        /// An example value for a Boolean.
        /// </summary>
        bool Boolean { get; set; }
        /// <summary>
        /// An example value for an Integer.
        /// </summary>
        int Integer { get; set; }
        /// <summary>
        /// An example value for a Float.
        /// </summary>
        float Float { get; set; }
        /// <summary>
        /// An example value for a Double.
        /// </summary>
        double Double { get; set; }

        /// <summary>
        /// An example value for a Date.
        /// </summary>
        Func<DateTime> Date { get; set; }

        /// <summary>
        /// An example value for a DateTime.
        /// </summary>
        Func<DateTime> DateTime { get; set; }

        /// <summary>
        /// An example value for Bytes.
        /// </summary>
        byte[] Bytes { get; set; }

        /// <summary>
        /// An example value for a Object.
        /// </summary>
        object Object { get; set; }

        /// <summary>
        /// An example value for a String.
        /// </summary>
        string String { get; set; }
    }
}