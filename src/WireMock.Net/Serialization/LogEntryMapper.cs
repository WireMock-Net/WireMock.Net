using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Util;

namespace WireMock.Serialization
{
    internal static class LogEntryMapper
    {
        public static LogEntryModel Map(LogEntry logEntry)
        {
            var logRequestModel = new LogRequestModel
            {
                DateTime = logEntry.RequestMessage.DateTime,
                ClientIP = logEntry.RequestMessage.ClientIP,
                Path = logEntry.RequestMessage.Path,
                AbsolutePath = logEntry.RequestMessage.AbsolutePath,
                Url = logEntry.RequestMessage.Url,
                AbsoluteUrl = logEntry.RequestMessage.AbsoluteUrl,
                Query = logEntry.RequestMessage.Query,
                Method = logEntry.RequestMessage.Method,
                Headers = logEntry.RequestMessage.Headers,
                Cookies = logEntry.RequestMessage.Cookies
            };

            if (logEntry.RequestMessage.BodyData != null)
            {
                switch (logEntry.RequestMessage.BodyData.DetectedBodyType)
                {
                    case BodyType.String:
                        logRequestModel.Body = logEntry.RequestMessage.BodyData.BodyAsString;
                        break;

                    case BodyType.Json:
                        logRequestModel.Body = logEntry.RequestMessage.BodyData.BodyAsString; // In case of Json, do also save the Body as string (backwards compatible)
                        logRequestModel.BodyAsJson = logEntry.RequestMessage.BodyData.BodyAsJson;
                        break;

                    case BodyType.Bytes:
                        logRequestModel.BodyAsBytes = logEntry.RequestMessage.BodyData.BodyAsBytes;
                        break;
                }

                logRequestModel.DetectedBodyType = logEntry.RequestMessage.BodyData.DetectedBodyType.ToString();
                logRequestModel.DetectedBodyTypeFromContentType = logEntry.RequestMessage.BodyData.DetectedBodyTypeFromContentType.ToString();

                logRequestModel.BodyEncoding = logEntry.RequestMessage.BodyData.Encoding != null
                    ? new EncodingModel
                    {
                        EncodingName = logEntry.RequestMessage.BodyData.Encoding.EncodingName,
                        CodePage = logEntry.RequestMessage.BodyData.Encoding.CodePage,
                        WebName = logEntry.RequestMessage.BodyData.Encoding.WebName
                    }
                    : null;
            }

            var logResponseModel = new LogResponseModel
            {
                StatusCode = logEntry.ResponseMessage.StatusCode,
                BodyDestination = logEntry.ResponseMessage.BodyDestination,
                Body = logEntry.ResponseMessage.Body,
                BodyAsJson = logEntry.ResponseMessage.BodyAsJson,
                BodyAsBytes = logEntry.ResponseMessage.BodyAsBytes,
                BodyOriginal = logEntry.ResponseMessage.BodyOriginal,
                BodyAsFile = logEntry.ResponseMessage.BodyAsFile,
                BodyAsFileIsCached = logEntry.ResponseMessage.BodyAsFileIsCached,
                Headers = logEntry.ResponseMessage.Headers,
                BodyEncoding = logEntry.ResponseMessage.BodyEncoding != null
                    ? new EncodingModel
                    {
                        EncodingName = logEntry.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.ResponseMessage.BodyEncoding.CodePage,
                        WebName = logEntry.ResponseMessage.BodyEncoding.WebName
                    }
                    : null
            };

            return new LogEntryModel
            {
                Guid = logEntry.Guid,
                MappingGuid = logEntry.MappingGuid,
                MappingTitle = logEntry.MappingTitle,
                Request = logRequestModel,
                Response = logResponseModel,
                RequestMatchResult = logEntry.RequestMatchResult != null ? new LogRequestMatchModel
                {
                    TotalScore = logEntry.RequestMatchResult.TotalScore,
                    TotalNumber = logEntry.RequestMatchResult.TotalNumber,
                    IsPerfectMatch = logEntry.RequestMatchResult.IsPerfectMatch,
                    AverageTotalScore = logEntry.RequestMatchResult.AverageTotalScore,
                    MatchDetails = logEntry.RequestMatchResult.MatchDetails.Select(x => new
                    {
                        Name = x.Key.Name.Replace("RequestMessage", string.Empty),
                        Score = x.Value
                    } as object).ToList()
                } : null
            };
        }
    }
}