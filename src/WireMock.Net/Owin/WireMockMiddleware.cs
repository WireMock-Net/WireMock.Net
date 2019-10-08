using System;
using System.Threading.Tasks;
using WireMock.Logging;
using System.Linq;
using WireMock.Matchers;
using Newtonsoft.Json;
using WireMock.Http;
using WireMock.Owin.Mappers;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.Validation;
#if !USE_ASPNETCORE
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
#else
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
#endif

namespace WireMock.Owin
{
    internal class WireMockMiddleware : OwinMiddleware
    {
        private static readonly Task CompletedTask = Task.FromResult(false);
        private readonly IWireMockMiddlewareOptions _options;
        private readonly IOwinRequestMapper _requestMapper;
        private readonly IOwinResponseMapper _responseMapper;
        private readonly IMappingMatcher _mappingMatcher;

#if !USE_ASPNETCORE
        public WireMockMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinRequestMapper requestMapper, IOwinResponseMapper responseMapper, IMappingMatcher mappingMatcher) : base(next)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(requestMapper, nameof(requestMapper));
            Check.NotNull(responseMapper, nameof(responseMapper));
            Check.NotNull(mappingMatcher, nameof(mappingMatcher));

            _options = options;
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _mappingMatcher = mappingMatcher;
        }
#else
        public WireMockMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinRequestMapper requestMapper, IOwinResponseMapper responseMapper, IMappingMatcher mappingMatcher)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(requestMapper, nameof(requestMapper));
            Check.NotNull(responseMapper, nameof(responseMapper));
            Check.NotNull(mappingMatcher, nameof(mappingMatcher));

            _options = options;
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _mappingMatcher = mappingMatcher;
        }
#endif

#if !USE_ASPNETCORE
        public override Task Invoke(IContext ctx)
#else
        public Task Invoke(IContext ctx)
#endif
        {
            return InvokeInternal(ctx);
        }

        private async Task InvokeInternal(IContext ctx)
        {
            var request = await _requestMapper.MapAsync(ctx.Request);

            bool logRequest = false;
            ResponseMessage response = null;
            MappingMatcherResult result = null;
            try
            {
                foreach (var mapping in _options.Mappings.Values.Where(m => m?.Scenario != null))
                {
                    // Set start
                    if (!_options.Scenarios.ContainsKey(mapping.Scenario) && mapping.IsStartState)
                    {
                        _options.Scenarios.TryAdd(mapping.Scenario, new ScenarioState
                        {
                            Name = mapping.Scenario
                        });
                    }
                }

                result = _mappingMatcher.FindBestMatch(request);

                var targetMapping = result?.Mapping;
                if (targetMapping == null)
                {
                    logRequest = true;
                    _options.Logger.Warn("HttpStatusCode set to 404 : No matching mapping found");
                    response = ResponseMessageBuilder.Create("No matching mapping found", 404);
                    return;
                }

                logRequest = targetMapping.LogMapping;

                if (targetMapping.IsAdminInterface && _options.AuthorizationMatcher != null)
                {
                    bool present = request.Headers.TryGetValue(HttpKnownHeaderNames.Authorization, out WireMockList<string> authorization);
                    if (!present || _options.AuthorizationMatcher.IsMatch(authorization.ToString()) < MatchScores.Perfect)
                    {
                        _options.Logger.Error("HttpStatusCode set to 401");
                        response = ResponseMessageBuilder.Create(null, 401);
                        return;
                    }
                }

                if (!targetMapping.IsAdminInterface && _options.RequestProcessingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_options.RequestProcessingDelay.Value);
                }

                response = await targetMapping.ProvideResponseAsync(request);

                if (targetMapping.Scenario != null)
                {
                    _options.Scenarios[targetMapping.Scenario].NextState = targetMapping.NextState;
                    _options.Scenarios[targetMapping.Scenario].Started = true;
                    _options.Scenarios[targetMapping.Scenario].Finished = targetMapping.NextState == null;
                }
            }
            catch (Exception ex)
            {
                _options.Logger.Error($"Providing a Response for Mapping '{result?.Mapping?.Guid}' failed. HttpStatusCode set to 500. Exception: {ex}");
                response = ResponseMessageBuilder.Create(JsonConvert.SerializeObject(ex), 500);
            }
            finally
            {
                var log = new LogEntry
                {
                    Guid = Guid.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,
                    MappingGuid = result?.Mapping?.Guid,
                    MappingTitle = result?.Mapping?.Title,
                    RequestMatchResult = result?.RequestMatchResult
                };

                LogRequest(log, logRequest);

                await _responseMapper.MapAsync(response, ctx.Response);
            }

            await CompletedTask;
        }

        private void LogRequest(LogEntry entry, bool addRequest)
        {
            _options.Logger.DebugRequestResponse(LogEntryMapper.Map(entry), entry.RequestMessage.Path.StartsWith("/__admin/"));

            if (addRequest)
            {
                _options.LogEntries.Add(entry);
            }

            if (_options.MaxRequestLogCount != null)
            {
                var logEntries = _options.LogEntries.ToList();
                foreach (var logEntry in logEntries.OrderBy(le => le.RequestMessage.DateTime).Take(logEntries.Count - _options.MaxRequestLogCount.Value))
                {
                    _options.LogEntries.Remove(logEntry);
                }
            }

            if (_options.RequestLogExpirationDuration != null)
            {
                var checkTime = DateTime.UtcNow.AddHours(-_options.RequestLogExpirationDuration.Value);

                foreach (var logEntry in _options.LogEntries.ToList().Where(le => le.RequestMessage.DateTime < checkTime))
                {
                    _options.LogEntries.Remove(logEntry);
                }
            }
        }
    }
}