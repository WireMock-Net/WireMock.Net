using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Matchers;

namespace WireMock.Serialization
{
    internal static class MatcherMapper
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

            string[] stringPatterns = matcher.Patterns != null ? matcher.Patterns.Cast<string>().ToArray() : new[] { matcher.Pattern as string };
            MatchBehaviour matchBehaviour = matcher.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;

            switch (matcherName)
            {
                case "LinqMatcher":
                    return new LinqMatcher(matchBehaviour, stringPatterns);

                case "ExactMatcher":
                    return new ExactMatcher(matchBehaviour, stringPatterns);

                case "RegexMatcher":
                    return new RegexMatcher(matchBehaviour, stringPatterns, matcher.IgnoreCase == true);

                case "JsonMatcher":
                    return new JsonMatcher(matchBehaviour, matcher.Pattern);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(matchBehaviour, stringPatterns);

                case "XPathMatcher":
                    return new XPathMatcher(matchBehaviour, (string)matcher.Pattern);

                case "WildcardMatcher":
                    return new WildcardMatcher(matchBehaviour, stringPatterns, matcher.IgnoreCase == true);

                case "SimMetricsMatcher":
                    SimMetricType type = SimMetricType.Levenstein;
                    if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                    {
                        throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                    }

                    return new SimMetricsMatcher(matchBehaviour, (string)matcher.Pattern, type);

                default:
                    throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
            }
        }

        public static MatcherModel[] Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            return matchers?.Select(Map).Where(x => x != null).ToArray();
        }

        public static MatcherModel Map([CanBeNull] IMatcher matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            // If the matcher is a IStringMatcher, get the patterns.
            // If the matcher is a IValueMatcher, get the value (can be string or object).
            // Else empty array
            object[] patterns = matcher is IStringMatcher stringMatcher ?
                stringMatcher.GetPatterns().Cast<object>().ToArray() :
                matcher is IValueMatcher valueMatcher ? new[] { valueMatcher.Value } :
                new object[0];
            bool? ignorecase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : (bool?)null;
            bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : (bool?)null;

            return new MatcherModel
            {
                RejectOnMatch = rejectOnMatch,
                IgnoreCase = ignorecase,
                Name = matcher.Name,
                Pattern = patterns.Length == 1 ? patterns.First() : null,
                Patterns = patterns.Length > 1 ? patterns : null
            };
        }
    }
}