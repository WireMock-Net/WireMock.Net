using System.Linq;
using System.Text;
using WireMock.Matchers;
using WireMock.Util;

namespace WireMock.Server
{
    public partial class FluentMockServer
    {
        private readonly RegexMatcher _adminFilesFilenamePathMatcher = new RegexMatcher(MatchBehaviour.AcceptOnMatch, @"^\/__admin\/files\/.*$");
        private static readonly Encoding[] FileBodyIsString = { Encoding.UTF8, Encoding.ASCII };

        #region Files/{filename}
        private ResponseMessage FilePost(RequestMessage requestMessage)
        {
            string path = requestMessage.Path.Substring(AdminFiles.Length + 1);

            _fileSystemHandler.WriteFile(path, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create("File created");
        }
        private ResponseMessage FilePut(RequestMessage requestMessage)
        {
            string path = requestMessage.Path.Substring(AdminFiles.Length + 1);

            if (!_fileSystemHandler.FileExists(path))
            {
                _logger.Info("The file '{0}' does not exist, updating file will be skipped.", path);
                return ResponseMessageBuilder.Create("File is not found", 404);
            }

            _fileSystemHandler.WriteFile(path, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create("File updated");
        }

        private ResponseMessage FileGet(RequestMessage requestMessage)
        {
            string path = requestMessage.Path.Substring(AdminFiles.Length + 1);

            if (!_fileSystemHandler.FileExists(path))
            {
                _logger.Info("The file '{0}' does not exist.", path);
                return ResponseMessageBuilder.Create("File is not found", 404);
            }

            byte[] bytes = _fileSystemHandler.ReadFile(path);
            var response = new ResponseMessage
            {
                StatusCode = 200,
                BodyData = new BodyData
                {
                    BodyAsBytes = bytes,
                    DetectedBodyType = BodyType.Bytes,
                    DetectedBodyTypeFromContentType = BodyType.None
                }
            };

            if (BytesEncodingUtils.TryGetEncoding(bytes, out Encoding encoding) && FileBodyIsString.Select(x => x.Equals(encoding)).Any())
            {
                response.BodyData.DetectedBodyType = BodyType.String;
                response.BodyData.BodyAsString = encoding.GetString(bytes);
            }

            return response;
        }

        private ResponseMessage FileDelete(RequestMessage requestMessage)
        {
            string path = requestMessage.Path.Substring(AdminFiles.Length + 1);

            if (!_fileSystemHandler.FileExists(path))
            {
                _logger.Info("The file '{0}' does not exist.", path);
                return ResponseMessageBuilder.Create("File is not deleted", 404);
            }

            _fileSystemHandler.DeleteFile(path);
            return ResponseMessageBuilder.Create("File deleted.");
        }
        #endregion
    }
}