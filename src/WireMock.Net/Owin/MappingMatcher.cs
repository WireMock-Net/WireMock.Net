using System;
using System.Collections.Generic;
using System.Linq;
using WireMock.Extensions;
using Stef.Validation;

namespace WireMock.Owin;

internal class MappingMatcher : IMappingMatcher
{
    private readonly IWireMockMiddlewareOptions _options;

    public MappingMatcher(IWireMockMiddlewareOptions options)
    {
        _options = Guard.NotNull(options);
    }

    public (MappingMatcherResult? Match, MappingMatcherResult? Partial) FindBestMatch(RequestMessage request)
    {
        Guard.NotNull(request);

        var possibleMappings = new List<MappingMatcherResult>();

        foreach (var mapping in _options.Mappings.Values.Where(m => m.TimeSettings.IsValid()))
        {
            try
            {
                var nextState = GetNextState(mapping);

                possibleMappings.Add(new MappingMatcherResult
                {
                    Mapping = mapping,
                    RequestMatchResult = mapping.GetRequestMatchResult(request, nextState)
                });
            }
            catch (Exception ex)
            {
                _options.Logger.Error($"Getting a Request MatchResult for Mapping '{mapping.Guid}' failed. This mapping will not be evaluated. Exception: {ex}");
            }
        }

        var partialMappings = possibleMappings
            .Where(pm => (pm.Mapping.IsAdminInterface && pm.RequestMatchResult.IsPerfectMatch) || !pm.Mapping.IsAdminInterface)
            .OrderBy(m => m.RequestMatchResult).ThenBy(m => m.Mapping.Priority).ThenByDescending(m => m.Mapping.UpdatedAt)
            .ToList();
        var partialMatch = partialMappings.FirstOrDefault(pm => pm.RequestMatchResult.AverageTotalScore > 0.0);

        if (_options.AllowPartialMapping == true)
        {
            return (partialMatch, partialMatch);
        }

        var match = possibleMappings
            .Where(m => m.RequestMatchResult.IsPerfectMatch)
            .OrderBy(m => m.Mapping.Priority).ThenBy(m => m.RequestMatchResult).ThenByDescending(m => m.Mapping.UpdatedAt)
            .FirstOrDefault();

        return (match, partialMatch);
    }

    private string? GetNextState(IMapping mapping)
    {
        // If the mapping does not have a scenario or _options.Scenarios does not contain this scenario from the mapping,
        // just return null to indicate that there is no next state.
        if (mapping.Scenario == null || !_options.Scenarios.ContainsKey(mapping.Scenario))
        {
            return null;
        }

        // Else just return the next state
        return _options.Scenarios[mapping.Scenario].NextState;
    }
}