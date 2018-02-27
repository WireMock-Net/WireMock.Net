using log4net;
using WireMock.Logging;

namespace WireMock.Net.StandAlone.NETCoreApp
{
    internal class WireMockLog4NetLogger : IWireMockLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public void Debug(string formatString, params object[] args)
        {
            Log.DebugFormat(formatString, args);
        }

        public void Info(string formatString, params object[] args)
        {
            Log.InfoFormat(formatString, args);
        }

        public void Warn(string formatString, params object[] args)
        {
            Log.WarnFormat(formatString, args);
        }

        public void Error(string formatString, params object[] args)
        {
            Log.ErrorFormat(formatString, args);
        }
    }
}