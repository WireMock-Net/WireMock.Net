// Copyright © WireMock.Net

using Newtonsoft.Json;
using RestEase;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WireMock.Client;

namespace WireMock.Net.Client.Net472
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
        }
    }

    //public interface IWireMockAdminApi
    //{
    //    /// <summary>
    //    /// Authentication header
    //    /// </summary>
    //    [Header("Authorization")]
    //    AuthenticationHeaderValue Authorization { get; set; }
    //}
}