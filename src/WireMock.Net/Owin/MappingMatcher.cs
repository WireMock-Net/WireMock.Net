using System.Linq;
using WireMock.Matchers.Request;
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

        public (Mapping Mapping, RequestMatchResult RequestMatchResult) Match(RequestMessage request)
        {
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

                return (bestPartialMatch?.Mapping, bestPartialMatch?.MatchResult);
            }
            else
            {
                var perfectMatch = mappings
                    .OrderBy(m => m.Mapping.Priority)
                    .FirstOrDefault(m => m.MatchResult.IsPerfectMatch);

                return (perfectMatch?.Mapping, perfectMatch?.MatchResult);
            }
        }
    }
}
