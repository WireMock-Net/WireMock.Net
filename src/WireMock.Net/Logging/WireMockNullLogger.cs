namespace WireMock.Logging
{
    /// <inheritdoc />
    /// <summary>
    /// WireMockNullLogger which does not log.
    /// </summary>
    /// <seealso cref="T:WireMock.Logging.IWireMockLogger" />
    public class WireMockNullLogger : IWireMockLogger
    {
        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Debug(string, object[])</cref>
        /// </see>
        public void Debug(string formatString, params object[] args)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Info(string, object[])</cref>
        /// </see>
        public void Info(string formatString, params object[] args)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Warn(string, object[])</cref>
        /// </see>
        public void Warn(string formatString, params object[] args)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Error(string, object[])</cref>
        /// </see>
        public void Error(string formatString, params object[] args)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Debug(string)</cref>
        /// </see>
        public void Debug(string message)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Info(string)</cref>
        /// </see>
        public void Info(string message)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Warn(string)</cref>
        /// </see>
        public void Warn(string message)
        {
        }

        /// <inheritdoc />
        /// <see>
        ///     <cref>IWireMockLogger.Error(string)</cref>
        /// </see>
        public void Error(string message)
        {
        }
    }
}