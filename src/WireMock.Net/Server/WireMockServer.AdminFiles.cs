using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WireMock.Matchers;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Server
{
    public partial class WireMockServer
    {
        private readonly RegexMatcher _adminFilesFilenamePathMatcher = new RegexMatcher(@"^\/__admin\/files\/.*$");
        private static readonly Encoding[] FileBodyIsString = { Encoding.UTF8, Encoding.ASCII };

        #region Files/{filename}
        private IResponseMessage FilePost(IRequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            string mappingFolder = _settings.FileSystemHandler.GetMappingFolder();
            if (!_settings.FileSystemHandler.FolderExists(mappingFolder))
            {
                _settings.FileSystemHandler.CreateFolder(mappingFolder);
            }

            _settings.FileSystemHandler.WriteFile(filename, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create(200, "File created");
        }

        private IResponseMessage FilePut(IRequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist, updating file will be skipped.", filename);
                return ResponseMessageBuilder.Create(404, "File is not found");
            }

            _settings.FileSystemHandler.WriteFile(filename, requestMessage.BodyAsBytes);

            return ResponseMessageBuilder.Create(200, "File updated");
        }

        private IResponseMessage FileGet(IRequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create(404, "File is not found");
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

            if (BytesEncodingUtils.TryGetEncoding(bytes, out var encoding) && FileBodyIsString.Select(x => x.Equals(encoding)).Any())
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
        private IResponseMessage FileHead(IRequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create(HttpStatusCode.NotFound);
            }

            return ResponseMessageBuilder.Create(HttpStatusCode.NoContent);
        }

        private IResponseMessage FileDelete(IRequestMessage requestMessage)
        {
            string filename = GetFileNameFromRequestMessage(requestMessage);

            if (!_settings.FileSystemHandler.FileExists(filename))
            {
                _settings.Logger.Info("The file '{0}' does not exist.", filename);
                return ResponseMessageBuilder.Create(404, "File is not deleted");
            }

            _settings.FileSystemHandler.DeleteFile(filename);
            return ResponseMessageBuilder.Create(200, "File deleted.");
        }

        private string GetFileNameFromRequestMessage(IRequestMessage requestMessage)
        {
            return Path.GetFileName(requestMessage.Path.Substring(AdminFiles.Length + 1));
        }
        #endregion
    }
}