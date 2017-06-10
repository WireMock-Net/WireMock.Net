using System;

namespace WireMock.Net.StandAlone.NETCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            StandAloneApp.Start(args);

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}