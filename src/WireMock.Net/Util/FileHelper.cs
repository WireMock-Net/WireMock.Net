using System.IO;
using System.Threading;
using JetBrains.Annotations;
using WireMock.Handlers;
using WireMock.Validation;

namespace WireMock.Util
{
    internal static class FileHelper
    {
        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 500;

        public static string ReadAllTextWithRetryAndDelay([NotNull] IFileSystemHandler filehandler, [NotNull] string path)
        {
            Check.NotNull(filehandler, nameof(filehandler));
            Check.NotNullOrEmpty(path, nameof(path));

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    return filehandler.ReadMappingFile(path);
                }
                catch
                {
                    Thread.Sleep(DelayOnRetry);
                }
            }

            throw new IOException();
        }
    }
}