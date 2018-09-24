using System;
using System.Threading.Tasks;
using WireMock.Logging;
using WireMock.Matchers.Request;
using System.Linq;
using WireMock.Matchers;
using WireMock.Util;
using Newtonsoft.Json;
using WireMock.Http;
using WireMock.Serialization;
#if !USE_ASPNETCORE
using Microsoft.Owin;
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
#else
using Microsoft.AspNetCore.Http;
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
#endif

namespace WireMock.Owin
{
    internal class WireMockMiddleware : OwinMiddleware
    {
        private static readonly Task CompletedTask = Task.FromResult(false);
        private readonly WireMockMiddlewareOptions _options;

        private readonly OwinRequestMapper _requestMapper = new OwinRequestMapper();
        private readonly OwinResponseMapper _responseMapper = new OwinResponseMapper();

#if !USE_ASPNETCORE
        public WireMockMiddleware(Next next, WireMockMiddlewareOptions options) : base(next)
        {
            _options = options;
        }
#else
        public WireMockMiddleware(Next next, WireMockMiddlewareOptions options)
        {
            _options = options;
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
            Mapping targetMapping = null;
            RequestMatchResult requestMatchResult = null;
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

                var mappings = _options.Mappings.Values
                    .Select(m => new
                    {
                        Mapping = m,
                        MatchResult = m.GetRequestMatchResult(request, m.Scenario != null && _options.Scenarios.ContainsKey(m.Scenario) ? _options.Scenarios[m.Scenario].NextState : null)
                    })
                    .ToList();

                if (_options.AllowPartialMapping)
                {
                    var partialMappings = mappings
                        .Where(pm => pm.Mapping.IsAdminInterface && pm.MatchResult.IsPerfectMatch || !pm.Mapping.IsAdminInterface)
                        .OrderBy(m => m.MatchResult)
                        .ThenBy(m => m.Mapping.Priority)
                        .ToList();

                    var bestPartialMatch = partialMappings.FirstOrDefault(pm => pm.MatchResult.AverageTotalScore > 0.0);

                    targetMapping = bestPartialMatch?.Mapping;
                    requestMatchResult = bestPartialMatch?.MatchResult;
                }
                else
                {
                    var perfectMatch = mappings
                        .OrderBy(m => m.Mapping.Priority)
                        .FirstOrDefault(m => m.MatchResult.IsPerfectMatch);

                    targetMapping = perfectMatch?.Mapping;
                    requestMatchResult = perfectMatch?.MatchResult;
                }

                if (targetMapping == null)
                {
                    logRequest = true;
                    _options.Logger.Warn("HttpStatusCode set to 404 : No matching mapping found");
                    response = ResponseMessageBuilder.Create("No matching mapping found", 404);
                    return;
                }

                logRequest = !targetMapping.IsAdminInterface;

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

                response = await targetMapping.ResponseToAsync(request);

                if (targetMapping.Scenario != null)
                {
                    _options.Scenarios[targetMapping.Scenario].NextState = targetMapping.NextState;
                    _options.Scenarios[targetMapping.Scenario].Started = true;
                    _options.Scenarios[targetMapping.Scenario].Finished = targetMapping.NextState == null;
                }
            }
            catch (Exception ex)
            {
                _options.Logger.Error("HttpStatusCode set to 500");
                response = ResponseMessageBuilder.Create(JsonConvert.SerializeObject(ex), 500);
            }
            finally
            {
                var log = new LogEntry
                {
                    Guid = Guid.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,
                    MappingGuid = targetMapping?.Guid,
                    MappingTitle = targetMapping?.Title,
                    RequestMatchResult = requestMatchResult
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
                var amount = _options.LogEntries.Count - _options.MaxRequestLogCount.Value;
                for (int i = 0; i < amount; i++)
                {
                    _options.LogEntries.RemoveAt(0);
                }
            }

            if (_options.RequestLogExpirationDuration != null)
            {
                var checkTime = DateTime.Now.AddHours(-_options.RequestLogExpirationDuration.Value);

                for (var i = _options.LogEntries.Count - 1; i >= 0; i--)
                {
                    var le = _options.LogEntries[i];
                    if (le.RequestMessage.DateTime <= checkTime)
                    {
                        _options.LogEntries.RemoveAt(i);
                    }
                }
            }
        }
    }
}