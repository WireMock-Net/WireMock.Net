using System.Linq;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Serialization
{
    internal static class LogEntryMapper
    {
        public static LogEntryModel Map(LogEntry logEntry)
        {
            return new LogEntryModel
            {
                Guid = logEntry.Guid,
                Request = new LogRequestModel
                {
                    DateTime = logEntry.RequestMessage.DateTime,
                    ClientIP = logEntry.RequestMessage.ClientIP,
                    Path = logEntry.RequestMessage.Path,
                    AbsolutePath = logEntry.RequestMessage.AbsolutePath,
                    Url = logEntry.RequestMessage.Url,
                    AbsoluteUrl = logEntry.RequestMessage.AbsoluteUrl,
                    Query = logEntry.RequestMessage.Query,
                    Method = logEntry.RequestMessage.Method,
                    Body = logEntry.RequestMessage.Body,
                    BodyAsJson = logEntry.RequestMessage.BodyAsJson,
                    BodyAsBytes = logEntry.RequestMessage.BodyAsBytes,
                    Headers = logEntry.RequestMessage.Headers,
                    Cookies = logEntry.RequestMessage.Cookies,
                    BodyEncoding = logEntry.RequestMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = logEntry.RequestMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.RequestMessage.BodyEncoding.CodePage,
                        WebName = logEntry.RequestMessage.BodyEncoding.WebName
                    } : null
                },
                Response = new LogResponseModel
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
                    BodyEncoding = logEntry.ResponseMessage.BodyEncoding != null ? new EncodingModel
                    {
                        EncodingName = logEntry.ResponseMessage.BodyEncoding.EncodingName,
                        CodePage = logEntry.ResponseMessage.BodyEncoding.CodePage,
                        WebName = logEntry.ResponseMessage.BodyEncoding.WebName
                    } : null
                },
                MappingGuid = logEntry.MappingGuid,
                MappingTitle = logEntry.MappingTitle,
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