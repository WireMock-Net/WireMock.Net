using System.IO;
using System.Linq;
using System.Text;
using WireMock.Matchers;
using WireMock.Types;
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
            string filename = GetFileNameFromRequestMessage(requestMessage);

            string mappingFolder = _settings.FileSystemHandler.GetMappingFolder();
            if (!_settings.FileSystemHandler.FolderExists(mappingFolder))
            {
                _settings.FileSystemHandler.CreateFolder(mappingFolder);
            }

            _settings.FileSystemHandler.WriteFile(filename, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create("File created");
        }

        private ResponseMessage FilePut(RequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist, updating file will be skipped.", filename);
                return ResponseMessageBuilder.Create("File is not found", 404);
            }

            _settings.FileSystemHandler.WriteFile(filename, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create("File updated");
        }

        private ResponseMessage FileGet(RequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create("File is not found", 404);
            }

            byte[] bytes = _settings.FileSystemHandler.ReadFile(filename);
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

        /// <summary>
        /// Checks if file exists.
        /// Note: Response is returned with no body as a head request doesn't accept a body, only the status code.
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        private ResponseMessage FileHead(RequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create(404);
            }

            return ResponseMessageBuilder.Create(204);
        }

        private ResponseMessage FileDelete(RequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create("File is not deleted", 404);
            }

            _settings.FileSystemHandler.DeleteFile(filename);
            return ResponseMessageBuilder.Create("File deleted.");
        }

        private string GetFileNameFromRequestMessage(RequestMessage requestMessage)
        {
            return Path.GetFileName(requestMessage.Path.Substring(AdminFiles.Length + 1));
        }
        #endregion
    }
}