using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Validation;

namespace WireMock.Owin
{
    internal class MappingMatcher : IMappingMatcher
    {
        private readonly IWireMockMiddlewareOptions _options;

        public MappingMatcher(IWireMockMiddlewareOptions options)
        {
            Check.NotNull(options, nameof(options));

            _options = options;
        }

        public (MappingMatcherResult Match, MappingMatcherResult Partial) FindBestMatch(RequestMessage request)
        {
            var mappings = new List<MappingMatcherResult>();
            foreach (var mapping in _options.Mappings.Values)
            {
                try
                {
                    string scenario = mapping.Scenario != null && _options.Scenarios.ContainsKey(mapping.Scenario) ? _options.Scenarios[mapping.Scenario].NextState : null;

                    mappings.Add(new MappingMatcherResult
                    {
                        Mapping = mapping,
                        RequestMatchResult = mapping.GetRequestMatchResult(request, scenario)
                    });
                }
                catch (Exception ex)
                {
                    _options.Logger.Error($"Getting a Request MatchResult for Mapping '{mapping.Guid}' failed. This mapping will not be evaluated. Exception: {ex}");
                }
            }

            var partialMappings = mappings
                .Where(pm => (pm.Mapping.IsAdminInterface && pm.RequestMatchResult.IsPerfectMatch) || !pm.Mapping.IsAdminInterface)
                .OrderBy(m => m.RequestMatchResult)
                .ThenBy(m => m.Mapping.Priority)
                .ToList();
            var partialMatch = partialMappings.FirstOrDefault(pm => pm.RequestMatchResult.AverageTotalScore > 0.0);

            if (_options.AllowPartialMapping == true)
            {
                return (partialMatch, partialMatch);
            }

            var match = mappings
                .Where(m => m.RequestMatchResult.IsPerfectMatch)
                .OrderBy(m => m.Mapping.Priority).ThenBy(m => m.RequestMatchResult)
                .FirstOrDefault();

            return (match, partialMatch);
        }
    }
}