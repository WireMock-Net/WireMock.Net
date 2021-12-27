using System;
using System.IO;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;

namespace WireMock.Net.OpenApiParser.ConsoleApp
{
    class Program
    {
        private const string Folder = "OpenApiFiles";
        static void Main(string[] args)
        {
            //RunOthersOpenApiParserExample();

            RunMockServerWithDynamicExampleGeneration();
        }

        private static void RunMockServerWithDynamicExampleGeneration() {
            //Run your mocking framework specifieing youur Example Values generator class.
            var serverCustomer_V2_json = Run.RunServer(Path.Combine(Folder, "Swagger_Customer_V2.0.json"), "http://localhost:8090/", true, new DynamicDataGeneration(), Types.ExampleValueType.Value, Types.ExampleValueType.Value);
            Console.WriteLine("Press any key to stop the servers");

            Console.ReadKey();
            serverCustomer_V2_json.Stop();
        }

        private static void RunOthersOpenApiParserExample()
        {
            var serverOpenAPIExamples = Run.RunServer(Path.Combine(Folder, "openAPIExamples.yaml"), "https://localhost:9091/");
            var serverPetstore_V2_json = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V2.0.json"), "https://localhost:9092/");
            var serverPetstore_V2_yaml = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V2.0.yaml"), "https://localhost:9093/");
            var serverPetstore_V300_yaml = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V3.0.0.yaml"), "https://localhost:9094/");
            var serverPetstore_V302_json = Run.RunServer(Path.Combine(Folder, "Swagger_Petstore_V3.0.2.json"), "https://localhost:9095/");

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();

            serverOpenAPIExamples.Stop();
            serverPetstore_V2_json.Stop();
            serverPetstore_V2_yaml.Stop();
            serverPetstore_V300_yaml.Stop();
            serverPetstore_V302_json.Stop();

            //IWireMockOpenApiParser parser = new WireMockOpenApiParser();

            //var petStoreModels = parser.FromStream(File.OpenRead("petstore-openapi3.json"), out OpenApiDiagnostic diagnostic1);
            //string petStoreJson = JsonConvert.SerializeObject(petStoreModels, Settings);
            // File.WriteAllText("../../../wiremock-petstore-openapi3.json", petStoreJson);

            //Run.RunServer(petStoreModels);

            //var mappingModels2 = parser.FromStream(File.OpenRead("infura.yaml"), out OpenApiDiagnostic diagnostic2);
            //Console.WriteLine(JsonConvert.SerializeObject(diagnostic2, Settings));

            //string json2 = JsonConvert.SerializeObject(mappingModels2, Settings);
            //Console.WriteLine(json2);
        }
    }
}