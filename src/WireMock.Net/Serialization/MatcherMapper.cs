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

            string[] patterns = matcher.Patterns ?? new[] { matcher.Pattern };
            MatchBehaviour matchBehaviour = matcher.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;

            switch (matcherName)
            {
                case "ExactMatcher":
                    return new ExactMatcher(matchBehaviour, patterns);

                case "RegexMatcher":
                    return new RegexMatcher(matchBehaviour, patterns, matcher.IgnoreCase == true);

                case "JsonObjectMatcher":
                    return new JsonObjectMatcher(matchBehaviour, matcher.Value);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(matchBehaviour, patterns);

                case "XPathMatcher":
                    return new XPathMatcher(matchBehaviour, matcher.Pattern);

                case "WildcardMatcher":
                    return new WildcardMatcher(matchBehaviour, patterns, matcher.IgnoreCase == true);

                case "SimMetricsMatcher":
                    SimMetricType type = SimMetricType.Levenstein;
                    if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                    {
                        throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                    }

                    return new SimMetricsMatcher(matchBehaviour, matcher.Pattern, type);

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

            string[] patterns = matcher is IStringMatcher stringMatcher ?
                stringMatcher.GetPatterns() :  new string[0];
            object value = matcher is IValueMatcher valueMatcher ? valueMatcher.GetValue() : null;
            bool? ignorecase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : (bool?)null;
            bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : (bool?)null;

            return new MatcherModel
            {
                RejectOnMatch = rejectOnMatch,
                IgnoreCase = ignorecase,
                Name = matcher.Name,
                Pattern = patterns.Length == 1 ? patterns.First() : null,
                Patterns = patterns.Length > 1 ? patterns : null,
                Value = value
            };
        }
    }
}