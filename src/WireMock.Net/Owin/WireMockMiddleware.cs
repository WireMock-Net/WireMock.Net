﻿using System;
using System.Collections;
using System.Threading.Tasks;
using WireMock.Logging;
using WireMock.Matchers.Request;
using System.Linq;
#if !NETSTANDARD
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace WireMock.Owin
{
#if !NETSTANDARD
    internal class WireMockMiddleware : OwinMiddleware
#else
    internal class WireMockMiddleware
#endif
    {
        private static readonly Task CompletedTask = Task.FromResult(false);
        private readonly WireMockMiddlewareOptions _options;

        private readonly OwinRequestMapper _requestMapper = new OwinRequestMapper();
        private readonly OwinResponseMapper _responseMapper = new OwinResponseMapper();

#if !NETSTANDARD
        public WireMockMiddleware(OwinMiddleware next, WireMockMiddlewareOptions options) : base(next)
        {
            _options = options;
        }
#else
        public WireMockMiddleware(RequestDelegate next, WireMockMiddlewareOptions options)
        {
            _options = options;
        }
#endif

#if !NETSTANDARD
        public override async Task Invoke(IOwinContext ctx)
#else
        public async Task Invoke(HttpContext ctx)
#endif
        {
            var request = await _requestMapper.MapAsync(ctx.Request);

            bool logRequest = false;
            ResponseMessage response = null;
            Mapping targetMapping = null;
            RequestMatchResult requestMatchResult = null;
            try
            {
                var mappings = _options.Mappings
                    .Select(m => new
                    {
                        Mapping = m,
                        MatchResult = m.IsRequestHandled(request)
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
                    response = new ResponseMessage { StatusCode = 404, Body = "No matching mapping found" };
                    return;
                }
                
                logRequest = !targetMapping.IsAdminInterface;

                if (targetMapping.IsAdminInterface && _options.AuthorizationMatcher != null)
                {
                    string authorization;
                    bool present = request.Headers.TryGetValue("Authorization", out authorization);
                    if (!present || _options.AuthorizationMatcher.IsMatch(authorization) < 1.0)
                    {
                        response = new ResponseMessage { StatusCode = 401 };
                        return;
                    }
                }

                if (!targetMapping.IsAdminInterface && _options.RequestProcessingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_options.RequestProcessingDelay.Value);
                }

                response = await targetMapping.ResponseToAsync(request);
            }
            catch (Exception ex)
            {
                response = new ResponseMessage { StatusCode = 500, Body = ex.ToString() };
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
            lock (((ICollection)_options.LogEntries).SyncRoot)
            {
                if (addRequest)
                {
                    _options.LogEntries.Add(entry);
                }

                if (_options.MaxRequestLogCount != null)
                {
                    _options.LogEntries = _options.LogEntries.Skip(_options.LogEntries.Count - _options.MaxRequestLogCount.Value).ToList();
                }

                if (_options.RequestLogExpirationDuration != null)
                {
                    var checkTime = DateTime.Now.AddHours(-_options.RequestLogExpirationDuration.Value);
                    _options.LogEntries = _options.LogEntries.Where(le => le.RequestMessage.DateTime > checkTime).ToList();
                }
            }
        }
    }
}