using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using System;
using System.IO;

namespace WireMock.Net.OpenApiParser.ConsoleApp
{
    class Program
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        static void Main(string[] args)
        {
            IWireMockOpenApiParser parser = new WireMockOpenApiParser();

            var petStoreModels = parser.FromStream(File.OpenRead("petstore.yml"), out OpenApiDiagnostic diagnostic1);
            Console.WriteLine(JsonConvert.SerializeObject(diagnostic1, Settings));

            string petStoreJson = JsonConvert.SerializeObject(petStoreModels, Settings);
            Console.WriteLine(petStoreJson);

            //var mappingModels2 = parser.FromStream(File.OpenRead("infura.yaml"), out OpenApiDiagnostic diagnostic2);
            //Console.WriteLine(JsonConvert.SerializeObject(diagnostic2, Settings));

            //string json2 = JsonConvert.SerializeObject(mappingModels2, Settings);
            //Console.WriteLine(json2);
        }
    }
}