using System.Linq;
using WireMock.Logging;
using WireMock.Models.Mappings;
using WireMock.Models.Requests;
using WireMock.Types;

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
                logRequestModel.DetectedBodyType = logEntry.RequestMessage.BodyData.DetectedBodyType.ToString();
                logRequestModel.DetectedBodyTypeFromContentType = logEntry.RequestMessage.BodyData.DetectedBodyTypeFromContentType.ToString();

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
                Headers = logEntry.ResponseMessage.Headers
            };

            if (logEntry.ResponseMessage.BodyData != null)
            {
                logResponseModel.BodyOriginal = logEntry.ResponseMessage.BodyOriginal;
                logResponseModel.BodyDestination = logEntry.ResponseMessage.BodyDestination;

                logResponseModel.DetectedBodyType = logEntry.ResponseMessage.BodyData.DetectedBodyType;
                logResponseModel.DetectedBodyTypeFromContentType = logEntry.ResponseMessage.BodyData.DetectedBodyTypeFromContentType;

                switch (logEntry.ResponseMessage.BodyData.DetectedBodyType)
                {
                    case BodyType.String:
                        logResponseModel.Body = logEntry.ResponseMessage.BodyData.BodyAsString;
                        break;

                    case BodyType.Json:
                        logResponseModel.BodyAsJson = logEntry.ResponseMessage.BodyData.BodyAsJson;
                        break;

                    case BodyType.Bytes:
                        logResponseModel.BodyAsBytes = logEntry.ResponseMessage.BodyData.BodyAsBytes;
                        break;

                    case BodyType.File:
                        logResponseModel.BodyAsFile = logEntry.ResponseMessage.BodyData.BodyAsFile;
                        logResponseModel.BodyAsFileIsCached = logEntry.ResponseMessage.BodyData.BodyAsFileIsCached;
                        break;
                }

                logResponseModel.BodyEncoding = logEntry.ResponseMessage.BodyData.Encoding != null
                    ? new EncodingModel
                    {
                        EncodingName = logEntry.ResponseMessage.BodyData.Encoding.EncodingName,
                        CodePage = logEntry.ResponseMessage.BodyData.Encoding.CodePage,
                        WebName = logEntry.ResponseMessage.BodyData.Encoding.WebName
                    }
                    : null;
            }

            return new LogEntryModel
            {
                Guid = logEntry.Guid,
                MappingGuid = logEntry.MappingGuid,
                MappingTitle = logEntry.MappingTitle,
                Request = logRequestModel,
                Response = logResponseModel,
                RequestMatchResult = logEntry.RequestMatchResult != null ? new LogRequestMatchModel
                {
                    IsPerfectMatch = logEntry.RequestMatchResult.IsPerfectMatch,
                    TotalScore = logEntry.RequestMatchResult.TotalScore,
                    TotalNumber = logEntry.RequestMatchResult.TotalNumber,
                    AverageTotalScore = logEntry.RequestMatchResult.AverageTotalScore,
                    MatchDetails = logEntry.RequestMatchResult.MatchDetails.Select(md => new
                    {
                        Name = md.MatcherType.Name.Replace("RequestMessage", string.Empty),
                        Score = md.Score
                    } as object).ToList()
                } : null
            };
        }
    }
}