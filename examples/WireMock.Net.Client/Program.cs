using Newtonsoft.Json;
using RestEase;
using System;
using System.Net.Http.Headers;
using System.Text;
using WireMock.Client;

namespace WireMock.Net.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an implementation of the IWireMockAdminApi and pass in the base URL for the API.
            var api = RestClient.For<IWireMockAdminApi>("http://localhost:9091");

            // Set BASIC Auth
            var value = Convert.ToBase64String(Encoding.ASCII.GetBytes("a:b"));
            api.Authorization = new AuthenticationHeaderValue("Basic", value);

            var settings1 = api.GetSettingsAsync().Result;
            Console.WriteLine($"settings1 = {JsonConvert.SerializeObject(settings1)}");

            settings1.GlobalProcessingDelay = 1077;
            api.PostSettingsAsync(settings1).Wait();

            var settings2 = api.GetSettingsAsync().Result;
            Console.WriteLine($"settings2 = {JsonConvert.SerializeObject(settings2)}");

            var mappings = api.GetMappingsAsync().Result;
            Console.WriteLine($"mappings = {JsonConvert.SerializeObject(mappings)}");

            try
            {
                var guid = Guid.Parse("11111110-a633-40e8-a244-5cb80bc0ab66");
                var mapping = api.GetMappingAsync(guid).Result;
                Console.WriteLine($"mapping = {JsonConvert.SerializeObject(mapping)}");
            }
            catch (Exception e)
            {
            }

            var request = api.GetRequestsAsync().Result;
            Console.WriteLine($"request = {JsonConvert.SerializeObject(request)}");

            //var deleteRequestsAsync = api.DeleteRequestsAsync().Result;
            //Console.WriteLine($"DeleteRequestsAsync = {deleteRequestsAsync.Status}");

            //var resetRequestsAsync = api.ResetRequestsAsync().Result;
            //Console.WriteLine($"ResetRequestsAsync = {resetRequestsAsync.Status}");

            var scenarioStates = api.GetScenariosAsync().Result;
            Console.WriteLine($"GetScenariosAsync = {JsonConvert.SerializeObject(scenarioStates)}");

            var postFileResult = api.PostFileAsync("1.cs", "C# Hello").GetAwaiter().GetResult();
            Console.WriteLine($"postFileResult = {JsonConvert.SerializeObject(postFileResult)}");

            var getFileResult = api.GetFileAsync("1.cs").GetAwaiter().GetResult();
            Console.WriteLine($"getFileResult = {getFileResult}");

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}