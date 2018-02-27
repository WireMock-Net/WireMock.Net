using System;

namespace WireMock.Logging
{
    /// <summary>
    /// WireMockConsoleLogger which logs to Console
    /// </summary>
    /// <seealso cref="IWireMockLogger" />
    public class WireMockConsoleLogger : IWireMockLogger
    {
        /// <see cref="IWireMockLogger.Debug"/>
        public void Debug(string formatString, params object[] args)
        {
            Console.WriteLine(Format("Debug", formatString, args));
        }

        /// <see cref="IWireMockLogger.Info"/>
        public void Info(string formatString, params object[] args)
        {
            Console.WriteLine(Format("Info", formatString, args));
        }

        /// <see cref="IWireMockLogger.Warn"/>
        public void Warn(string formatString, params object[] args)
        {
            Console.WriteLine(Format("Warn", formatString, args));
        }

        /// <see cref="IWireMockLogger.Error"/>
        public void Error(string formatString, params object[] args)
        {
            Console.WriteLine(Format("Error", formatString, args));
        }

        private static string Format(string level, string formatString, params object[] args)
        {
            string message = string.Format(formatString, args);

            return $"{DateTime.UtcNow} [{level}] : {message}";
        }
    }
}