using System;
using JetBrains.Annotations;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Matchers;

namespace WireMock.Serialization
{
    internal static class MatcherModelMapper
    {
        public static IMatcher Map([CanBeNull] MatcherModel matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            string[] parts = matcher.Name.Split('.');
            string matcherName = parts[0];
            string matcherType = parts.Length > 1 ? parts[1] : null;

            string[] patterns = matcher.Patterns ?? new[] { matcher.Pattern };

            switch (matcherName)
            {
                case "ExactMatcher":
                    return new ExactMatcher(patterns);

                case "RegexMatcher":
                    return new RegexMatcher(patterns, matcher.IgnoreCase == true);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(patterns);

                case "XPathMatcher":
                    return new XPathMatcher(matcher.Pattern);

                case "WildcardMatcher":
                    return new WildcardMatcher(patterns, matcher.IgnoreCase == true);

                case "SimMetricsMatcher":
                    SimMetricType type = SimMetricType.Levenstein;
                    if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                    {
                        throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                    }

                    return new SimMetricsMatcher(matcher.Pattern, type);

                default:
                    throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
            }
        }
    }
}
