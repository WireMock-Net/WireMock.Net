using System.IO;
using System.Threading;

namespace WireMock.Util
{
    internal static class FileHelper
    {
        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 500;

        public static string ReadAllText(string path)
        {
            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch
                {
                    // You may check error code to filter some exceptions, not every error can be recovered.
                    Thread.Sleep(DelayOnRetry);
                }
            }

            throw new IOException();
        }
    }
}