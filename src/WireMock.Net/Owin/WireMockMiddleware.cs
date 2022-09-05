using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Http;
using WireMock.Owin.Mappers;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.ResponseBuilders;
using WireMock.Settings;
#if !USE_ASPNETCORE
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
        private readonly object _lock = new();
        private static readonly Task CompletedTask = Task.FromResult(false);
        private readonly IWireMockMiddlewareOptions _options;
        private readonly IOwinRequestMapper _requestMapper;
        private readonly IOwinResponseMapper _responseMapper;
        private readonly IMappingMatcher _mappingMatcher;

#if !USE_ASPNETCORE
        public WireMockMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinRequestMapper requestMapper, IOwinResponseMapper responseMapper, IMappingMatcher mappingMatcher) : base(next)
        {
            _options = Guard.NotNull(options, nameof(options));
            _requestMapper = Guard.NotNull(requestMapper, nameof(requestMapper));
            _responseMapper = Guard.NotNull(responseMapper, nameof(responseMapper));
            _mappingMatcher = Guard.NotNull(mappingMatcher, nameof(mappingMatcher));
        }
#else
        public WireMockMiddleware(Next next, IWireMockMiddlewareOptions options, IOwinRequestMapper requestMapper, IOwinResponseMapper responseMapper, IMappingMatcher mappingMatcher)
        {
            _options = Guard.NotNull(options, nameof(options));
            _requestMapper = Guard.NotNull(requestMapper, nameof(requestMapper));
            _responseMapper = Guard.NotNull(responseMapper, nameof(responseMapper));
            _mappingMatcher = Guard.NotNull(mappingMatcher, nameof(mappingMatcher));
        }
#endif

#if !USE_ASPNETCORE
        public override Task Invoke(IContext ctx)
#else
        public Task Invoke(IContext ctx)
#endif
        {
            if (_options.HandleRequestsSynchronously.GetValueOrDefault(true))
            {
                lock (_lock)
                {
                    return InvokeInternalAsync(ctx);
                }
            }

            return InvokeInternalAsync(ctx);
        }

        private async Task InvokeInternalAsync(IContext ctx)
        {
            var request = await _requestMapper.MapAsync(ctx.Request, _options).ConfigureAwait(false);

            var logRequest = false;
            IResponseMessage? response = null;
            (MappingMatcherResult? Match, MappingMatcherResult? Partial) result = (null, null);

            try
            {
                foreach (var mapping in _options.Mappings.Values)
                {
                    if (mapping.Scenario is null)
                    {
                        continue;
                    }

                    // Set scenario start
                    if (!_options.Scenarios.ContainsKey(mapping.Scenario) && mapping.IsStartState)
                    {
                        _options.Scenarios.TryAdd(mapping.Scenario, new ScenarioState
                        {
                            Name = mapping.Scenario
                        });
                    }
                }

                result = _mappingMatcher.FindBestMatch(request);

                var targetMapping = result.Match?.Mapping;
                if (targetMapping == null)
                {
                    logRequest = true;
                    _options.Logger.Warn("HttpStatusCode set to 404 : No matching mapping found");
                    response = ResponseMessageBuilder.Create("No matching mapping found", 404);
                    return;
                }

                logRequest = targetMapping.LogMapping;

                if (targetMapping.IsAdminInterface && _options.AuthenticationMatcher != null && request.Headers != null)
                {
                    bool present = request.Headers.TryGetValue(HttpKnownHeaderNames.Authorization, out WireMockList<string> authorization);
                    if (!present || _options.AuthenticationMatcher.IsMatch(authorization!.ToString()) < MatchScores.Perfect)
                    {
                        _options.Logger.Error("HttpStatusCode set to 401");
                        response = ResponseMessageBuilder.Create(null, HttpStatusCode.Unauthorized);
                        return;
                    }
                }

                if (!targetMapping.IsAdminInterface && _options.RequestProcessingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_options.RequestProcessingDelay.Value).ConfigureAwait(false);
                }

                var (theResponse, theOptionalNewMapping) = await targetMapping.ProvideResponseAsync(request).ConfigureAwait(false);
                response = theResponse;

                var responseBuilder = targetMapping.Provider as Response;

                if (!targetMapping.IsAdminInterface && theOptionalNewMapping != null)
                {
                    if (responseBuilder?.ProxyAndRecordSettings?.SaveMapping == true || targetMapping.Settings?.ProxyAndRecordSettings?.SaveMapping == true)
                    {
                        _options.Mappings.TryAdd(theOptionalNewMapping.Guid, theOptionalNewMapping);
                    }

                    if (responseBuilder?.ProxyAndRecordSettings?.SaveMappingToFile == true || targetMapping.Settings?.ProxyAndRecordSettings?.SaveMappingToFile == true)
                    {
                        var matcherMapper = new MatcherMapper(targetMapping.Settings);
                        var mappingConverter = new MappingConverter(matcherMapper);
                        var mappingToFileSaver = new MappingToFileSaver(targetMapping.Settings, mappingConverter);

                        mappingToFileSaver.SaveMappingToFile(theOptionalNewMapping);
                    }
                }

                if (targetMapping.Scenario != null)
                {
                    UpdateScenarioState(targetMapping);
                }

                if (!targetMapping.IsAdminInterface && targetMapping.Webhooks?.Length > 0)
                {
                    await SendToWebhooksAsync(targetMapping, request, response).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _options.Logger.Error($"Providing a Response for Mapping '{result.Match?.Mapping?.Guid}' failed. HttpStatusCode set to 500. Exception: {ex}");
                response = ResponseMessageBuilder.Create(ex.Message, 500);
            }

            finally
            {
                var log = new LogEntry
                {
                    Guid = Guid.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,

                    MappingGuid = result.Match?.Mapping?.Guid,
                    MappingTitle = result.Match?.Mapping?.Title,
                    RequestMatchResult = result.Match?.RequestMatchResult,

                    PartialMappingGuid = result.Partial?.Mapping?.Guid,
                    PartialMappingTitle = result.Partial?.Mapping?.Title,
                    PartialMatchResult = result.Partial?.RequestMatchResult
                };

                LogRequest(log, logRequest);

                try
                {
                    if (_options.SaveUnmatchedRequests == true && result.Match?.RequestMatchResult.IsPerfectMatch != true)
                    {
                        var filename = $"{log.Guid}.LogEntry.json";
                        _options.FileSystemHandler?.WriteUnmatchedRequest(filename, Util.JsonUtils.Serialize(log));
                    }
                }
                catch
                {
                    // Empty catch
                }

                await _responseMapper.MapAsync(response, ctx.Response).ConfigureAwait(false);
            }

            await CompletedTask.ConfigureAwait(false);
        }

        private async Task SendToWebhooksAsync(IMapping mapping, IRequestMessage request, IResponseMessage response)
        {
            for (int index = 0; index < mapping.Webhooks?.Length; index++)
            {
                var httpClientForWebhook = HttpClientBuilder.Build(mapping.Settings.WebhookSettings ?? new WebhookSettings());
                var webhookSender = new WebhookSender(mapping.Settings);
                var webhookRequest = mapping.Webhooks[index].Request;

                try
                {
                    await webhookSender.SendAsync(httpClientForWebhook, mapping, webhookRequest, request, response).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _options.Logger.Error($"Sending message to Webhook [{index}] from Mapping '{mapping.Guid}' failed. Exception: {ex}");
                }
            }
        }

        private void UpdateScenarioState(IMapping mapping)
        {
            var scenario = _options.Scenarios[mapping.Scenario!];

            // Increase the number of times this state has been executed
            scenario.Counter++;

            // Only if the number of times this state is executed equals the required StateTimes, proceed to next state and reset the counter to 0
            if (scenario.Counter == (mapping.StateTimes ?? 1))
            {
                scenario.NextState = mapping.NextState;
                scenario.Counter = 0;
            }

            // Else just update Started and Finished
            scenario.Started = true;
            scenario.Finished = mapping.NextState == null;
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