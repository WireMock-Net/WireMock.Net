using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace WireMock.Net.StandAlone.Net461
{
    static class Program
    {
        static void Main(string[] args)
        {
            var server = StandAloneApp.Start(args);
            server.Given(Request.Create())
                .RespondWith(Response.Create().WithProxy("http://10.10.66.65"));

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}