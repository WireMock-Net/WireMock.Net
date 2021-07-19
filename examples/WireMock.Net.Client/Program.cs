using Newtonsoft.Json;
using RestEase;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WireMock.Client;

namespace WireMock.Net.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create an implementation of the IWireMockAdminApi and pass in the base URL for the API.
            var api = RestClient.For<IWireMockAdminApi>("http://localhost:9091");

            // Set BASIC Auth
            var value = Convert.ToBase64String(Encoding.ASCII.GetBytes("a:b"));
            api.Authorization = new AuthenticationHeaderValue("Basic", value);

            var settings1 = await api.GetSettingsAsync();
            Console.WriteLine($"settings1 = {JsonConvert.SerializeObject(settings1)}");

            var settingsViaBuilder = new FluentBuilder.SettingsModelBuilder()
                .WithGlobalProcessingDelay(1077)
                .WithoutGlobalProcessingDelay()
                .Build();

            settings1.GlobalProcessingDelay = 1077;
            api.PostSettingsAsync(settings1).Wait();

            var settings2 = await api.GetSettingsAsync();
            Console.WriteLine($"settings2 = {JsonConvert.SerializeObject(settings2)}");

            var mappings = await api.GetMappingsAsync();
            Console.WriteLine($"mappings = {JsonConvert.SerializeObject(mappings)}");

            try
            {
                var guid = Guid.Parse("11111110-a633-40e8-a244-5cb80bc0ab66");
                var mapping = await api.GetMappingAsync(guid);
                Console.WriteLine($"mapping = {JsonConvert.SerializeObject(mapping)}");
            }
            catch (Exception e)
            {
            }

            var request = await api.GetRequestsAsync();
            Console.WriteLine($"request = {JsonConvert.SerializeObject(request)}");

            //var deleteRequestsAsync = api.DeleteRequestsAsync().Result;
            //Console.WriteLine($"DeleteRequestsAsync = {deleteRequestsAsync.Status}");

            //var resetRequestsAsync = api.ResetRequestsAsync().Result;
            //Console.WriteLine($"ResetRequestsAsync = {resetRequestsAsync.Status}");

            var scenarioStates = await api.GetScenariosAsync();
            Console.WriteLine($"GetScenariosAsync = {JsonConvert.SerializeObject(scenarioStates)}");

            var postFileResult = await api.PostFileAsync("1.cs", "C# Hello");
            Console.WriteLine($"postFileResult = {JsonConvert.SerializeObject(postFileResult)}");

            var getFileResult = await api.GetFileAsync("1.cs");
            Console.WriteLine($"getFileResult = {getFileResult}");

            var resetMappingsAsync = await api.ResetMappingsAsync();
            Console.WriteLine($"resetMappingsAsync = {resetMappingsAsync.Status}");

            var resetMappingsAndReloadStaticMappingsAsync = await api.ResetMappingsAsync(true);
            Console.WriteLine($"resetMappingsAndReloadStaticMappingsAsync = {resetMappingsAndReloadStaticMappingsAsync.Status}");

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
    }
}