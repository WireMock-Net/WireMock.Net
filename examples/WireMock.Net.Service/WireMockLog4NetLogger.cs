// Copyright © WireMock.Net

using System;
using log4net;
using Newtonsoft.Json;
using Wiremock.Net.Service;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Net.Service
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

        public void Error(string message, Exception exception)
        {
            Log.Error(message, exception);
        }

        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
            Log.DebugFormat("Admin[{0}] {1}", isAdminRequest, message);
        }
    }
}