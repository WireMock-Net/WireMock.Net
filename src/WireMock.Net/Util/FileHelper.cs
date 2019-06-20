using JetBrains.Annotations;
using System.Threading;
using WireMock.Handlers;
using WireMock.Validation;

namespace WireMock.Util
{
    internal static class FileHelper
    {
        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 500;

        public static bool TryReadMappingFileWithRetryAndDelay([NotNull] IFileSystemHandler handler, [NotNull] string path, out string value)
        {
            Check.NotNull(handler, nameof(handler));
            Check.NotNullOrEmpty(path, nameof(path));

            value = null;

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    value = handler.ReadMappingFile(path);
                    return true;
                }
                catch
                {
                    Thread.Sleep(DelayOnRetry);
                }
            }

            return false;
        }
    }
}