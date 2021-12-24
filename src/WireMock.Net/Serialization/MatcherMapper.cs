using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using JetBrains.Annotations;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Plugin;
using WireMock.Settings;

namespace WireMock.Serialization
{
    internal class MatcherMapper
    {
        private readonly IWireMockServerSettings _settings;

        public MatcherMapper(IWireMockServerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public IMatcher[] Map([CanBeNull] IEnumerable<MatcherModel> matchers)
        {
            return matchers?.Select(Map).Where(m => m != null).ToArray();
        }

        public IMatcher Map([CanBeNull] MatcherModel matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            string[] parts = matcher.Name.Split('.');
            string matcherName = parts[0];
            string matcherType = parts.Length > 1 ? parts[1] : null;
            var stringPatterns = ParseStringPatterns(matcher);
            var matchBehaviour = matcher.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;
            bool ignoreCase = matcher.IgnoreCase == true;
            bool throwExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails == true;
            bool useRegexExtended = _settings.UseRegexExtended == true;

            switch (matcherName)
            {
                case nameof(NotNullOrEmptyMatcher):
                    return new NotNullOrEmptyMatcher(matchBehaviour);

                case "CSharpCodeMatcher":
                    if (_settings.AllowCSharpCodeMatcher == true)
                    {
                        return PluginLoader.Load<ICSharpCodeMatcher>(matchBehaviour, stringPatterns);
                    }

                    throw new NotSupportedException("It's not allowed to use the 'CSharpCodeMatcher' because IWireMockServerSettings.AllowCSharpCodeMatcher is not set to 'true'.");

                case nameof(LinqMatcher):
                    return new LinqMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case nameof(ExactMatcher):
                    return new ExactMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case nameof(ExactObjectMatcher):
                    return CreateExactObjectMatcher(matchBehaviour, stringPatterns[0], throwExceptionWhenMatcherFails);

                case nameof(RegexMatcher):
                    return new RegexMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails, useRegexExtended);

                case nameof(JsonMatcher):
                    object valueForJsonMatcher = matcher.Pattern ?? matcher.Patterns;
                    return new JsonMatcher(matchBehaviour, valueForJsonMatcher, ignoreCase, throwExceptionWhenMatcherFails);

                case nameof(JsonPartialMatcher):
                    object valueForJsonPartialMatcher = matcher.Pattern ?? matcher.Patterns;
                    return new JsonPartialMatcher(matchBehaviour, valueForJsonPartialMatcher, ignoreCase, throwExceptionWhenMatcherFails);

                case nameof(JsonPartialWildcardMatcher):
                    object valueForJsonPartialWildcardMatcher = matcher.Pattern ?? matcher.Patterns;
                    return new JsonPartialWildcardMatcher(matchBehaviour, valueForJsonPartialWildcardMatcher, ignoreCase, throwExceptionWhenMatcherFails);

                case nameof(JsonPathMatcher):
                    return new JsonPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case nameof(JmesPathMatcher):
                    return new JmesPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case nameof(XPathMatcher):
                    return new XPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case nameof(WildcardMatcher):
                    return new WildcardMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case nameof(ContentTypeMatcher):
                    return new ContentTypeMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case nameof(SimMetricsMatcher):
                    SimMetricType type = SimMetricType.Levenstein;
                    if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                    {
                        throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                    }

                    return new SimMetricsMatcher(matchBehaviour, stringPatterns, type, throwExceptionWhenMatcherFails);

                default:
                    throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
            }
        }

        public MatcherModel[] Map([CanBeNull] IEnumerable<IMatcher> matchers)
        {
            return matchers?.Select(Map).Where(m => m != null).ToArray();
        }

        public MatcherModel Map([CanBeNull] IMatcher matcher)
        {
            if (matcher == null)
            {
                return null;
            }

            bool? ignoreCase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : (bool?)null;
            bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : (bool?)null;

            var model = new MatcherModel
            {
                RejectOnMatch = rejectOnMatch,
                IgnoreCase = ignoreCase,
                Name = matcher.Name
            };

            switch (matcher)
            {
                // If the matcher is a IStringMatcher, get the patterns.
                case IStringMatcher stringMatcher:
                    var stringPatterns = stringMatcher.GetPatterns();
                    if (stringPatterns.Length == 1)
                    {
                        if (stringPatterns[0].IsFirst)
                        {
                            model.Pattern = stringPatterns[0].First;
                        }
                        else
                        {
                            model.Pattern = stringPatterns[0].Second.Pattern;
                            model.PatternAsFile = stringPatterns[0].Second.PatternAsFile;
                        }
                    }
                    else
                    {
                        model.Patterns = stringPatterns.Select(p => p.GetPattern()).Cast<object>().ToArray();
                    }
                    break;

                // If the matcher is a IValueMatcher, get the value (can be string or object).
                case IValueMatcher valueMatcher:
                    model.Patterns = new[] { valueMatcher.Value };
                    break;

                // If the matcher is a ExactObjectMatcher, get the ValueAsObject or ValueAsBytes.
                case ExactObjectMatcher exactObjectMatcher:
                    model.Patterns = new[] { exactObjectMatcher.ValueAsObject ?? exactObjectMatcher.ValueAsBytes };
                    break;
            }

            return model;
        }

        private AnyOf<string, StringPattern>[] ParseStringPatterns(MatcherModel matcher)
        {
            if (matcher.Pattern is string patternAsString)
            {
                return new[] { new AnyOf<string, StringPattern>(patternAsString) };
            }

            if (matcher.Pattern is IEnumerable<string> patternAsStringArray)
            {
                return patternAsStringArray.ToAnyOfPatterns();
            }

            if (matcher.Patterns?.OfType<string>() is IEnumerable<string> patternsAsStringArray)
            {
                return patternsAsStringArray.ToAnyOfPatterns();
            }

            if (!string.IsNullOrEmpty(matcher.PatternAsFile))
            {
                var pattern = _settings.FileSystemHandler.ReadFileAsString(matcher.PatternAsFile);
                return new[] { new AnyOf<string, StringPattern>(new StringPattern { Pattern = pattern, PatternAsFile = matcher.PatternAsFile }) };
            }

            return new AnyOf<string, StringPattern>[0];
        }

        private ExactObjectMatcher CreateExactObjectMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> stringPattern, bool throwException)
        {
            byte[] bytePattern;
            try
            {
                bytePattern = Convert.FromBase64String(stringPattern.GetPattern());
            }
            catch
            {
                throw new ArgumentException($"Matcher 'ExactObjectMatcher' has invalid pattern. The pattern value '{stringPattern}' is not a Base64String.", nameof(stringPattern));
            }

            return new ExactObjectMatcher(matchBehaviour, bytePattern, throwException);
        }
    }
}