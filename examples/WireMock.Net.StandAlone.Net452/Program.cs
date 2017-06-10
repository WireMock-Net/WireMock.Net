using System;

namespace WireMock.Net.StandAlone
{
    public class Program
    {
        static void Main(params string[] args)
        {
            StandAloneApp.Start(args);

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}