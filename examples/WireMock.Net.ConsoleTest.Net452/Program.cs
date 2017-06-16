using System;
using WireMock.Net.StandAlone;

namespace WireMock.Net.ConsoleTest.Net452
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