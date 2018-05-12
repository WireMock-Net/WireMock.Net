using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WireMock.Admin.Mappings;
using WireMock.Matchers;

namespace WireMock.Serialization
{
    internal static class MatcherMapper
    {
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

            string[] patterns = matcher is IStringMatcher stringMatcher ? stringMatcher.GetPatterns() : new string[0];
            bool? ignorecase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : (bool?)null;
            bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : (bool?) null;

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