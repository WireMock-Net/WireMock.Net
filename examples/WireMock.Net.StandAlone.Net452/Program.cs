using System;
using System.IO;
using log4net.Config;

namespace WireMock.Net.StandAlone.Net452
{
    public class Program
    {
        static void Main(params string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            StandAloneApp.Start(args);

            Console.WriteLine("Press any key to stop the server");
            Console.ReadKey();
        }
    }
}