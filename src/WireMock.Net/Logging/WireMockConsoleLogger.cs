using System;

namespace WireMock.Logging
{
    /// <inheritdoc />
    /// <summary>
    /// WireMockConsoleLogger which logs to Console
    /// </summary>
    /// <seealso cref="T:WireMock.Logging.IWireMockLogger" />
    public class WireMockConsoleLogger : IWireMockLogger
    {
        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Debug(string, object[])</cref>
        /// </see>
        public void Debug(string formatString, params object[] args) => Console.WriteLine(Format("Debug", formatString, args));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Info(string, object[])</cref>
        /// </see>
        public void Info(string formatString, params object[] args) => Console.WriteLine(Format("Info", formatString, args));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Warn(string, object[])</cref>
        /// </see>
        public void Warn(string formatString, params object[] args) => Console.WriteLine(Format("Warn", formatString, args));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Error(string, object[])</cref>
        /// </see>
        public void Error(string formatString, params object[] args) => Console.WriteLine(Format("Error", formatString, args));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Debug(string)</cref>
        /// </see>
        public void Debug(string message) => Console.WriteLine(Log("Debug", message));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Info(string)</cref>
        /// </see>
        public void Info(string message) => Console.WriteLine(Log("Info", message));

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Warn(string)</cref>
        /// </see>
        public void Warn(string message) => Console.WriteLine(Log("Warn", message));
        
        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Error(string)</cref>
        /// </see>
        public void Error(string message) => Console.WriteLine(Log("Error", message));
        
        private static string Format(string level, string formatString, params object[] args)
        {
            var message = string.Format(formatString, args);

            return Log(level, message);
        }

        private static string Log(string level, string message)
        {
            return $"{DateTime.UtcNow} [{level}] : {message}";
        }
    }
}