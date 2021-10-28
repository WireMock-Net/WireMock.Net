using System;

namespace WireMock.Net.OpenApiParser.Settings
{
    /// <summary>
    /// A class defining the example values to use for the different types.
    /// </summary>
    public interface IWireMockOpenApiParserExampleValues
    {
#pragma warning disable 1591
        bool Boolean { get; set; }        
        int Integer { get; set; }        
        float Float { get; set; }        

        double Double { get; set; }        

        Func<DateTime> Date { get; set; }

        Func<DateTime> DateTime { get; set; }        

        byte[] Bytes { get; set; }        

        object Object { get; set; }

        string String { get; set; }
#pragma warning restore 1591
    }
}