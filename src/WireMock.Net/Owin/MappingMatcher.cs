// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Services;

namespace WireMock.Owin;

internal class MappingMatcher : IMappingMatcher
{
    private readonly IWireMockMiddlewareOptions _options;
    private readonly IRandomizerDoubleBetween0And1 _randomizerDoubleBetween0And1;

    public MappingMatcher(IWireMockMiddlewareOptions options, IRandomizerDoubleBetween0And1 randomizerDoubleBetween0And1)
    {
        _options = Guard.NotNull(options);
        _randomizerDoubleBetween0And1 = Guard.NotNull(randomizerDoubleBetween0And1);
    }

    public (MappingMatcherResult? Match, MappingMatcherResult? Partial) FindBestMatch(RequestMessage request)
    {
        Guard.NotNull(request);

        var possibleMappings = new List<MappingMatcherResult>();

        var mappings = _options.Mappings.Values
            .Where(m => m.TimeSettings.IsValid())
            .Where(m => m.Probability is null || m.Probability <= _randomizerDoubleBetween0And1.Generate())
            .ToArray();

        foreach (var mapping in mappings)
        {
            try
            {
                var nextState = GetNextState(mapping);

                var mappingMatcherResult = new MappingMatcherResult(mapping, mapping.GetRequestMatchResult(request, nextState));

                var exceptions = mappingMatcherResult.RequestMatchResult.MatchDetails
                    .Where(md => md.Exception != null)
                    .Select(md => md.Exception)
                    .ToArray();

                if (!exceptions.Any())
                {
                    possibleMappings.Add(mappingMatcherResult);
                }
                else if (!request.AbsolutePath.StartsWith("/__admin", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var ex in exceptions)
                    {
                        LogException(mapping, ex!);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(mapping, ex);
            }
        }

        var partialMappings = possibleMappings
            .Where(pm => (pm.Mapping.IsAdminInterface && pm.RequestMatchResult.IsPerfectMatch) || !pm.Mapping.IsAdminInterface)
            .OrderBy(m => m.RequestMatchResult)
                .ThenBy(m => m.RequestMatchResult.TotalNumber)
                .ThenBy(m => m.Mapping.Priority)
                .ThenByDescending(m => m.Mapping.UpdatedAt)
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

    private void LogException(IMapping mapping, Exception ex)
    {
        _options.Logger.Error($"Getting a Request MatchResult for Mapping '{mapping.Guid}' failed. This mapping will not be evaluated. Exception: {ex}");
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