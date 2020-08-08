﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SimMetrics.Net;
using WireMock.Admin.Mappings;
using WireMock.Matchers;
using WireMock.Settings;
using WireMock.Validation;

namespace WireMock.Serialization
{
    internal class MatcherMapper
    {
        private readonly IWireMockServerSettings _settings;

        public MatcherMapper(IWireMockServerSettings settings)
        {
            Check.NotNull(settings, nameof(settings));
            _settings = settings;
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

            string[] stringPatterns = matcher.Patterns != null ? matcher.Patterns.OfType<string>().ToArray() : new[] { matcher.Pattern as string };
            MatchBehaviour matchBehaviour = matcher.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;
            bool ignoreCase = matcher.IgnoreCase == true;
            bool throwExceptionWhenMatcherFails = _settings.ThrowExceptionWhenMatcherFails == true;

            switch (matcherName)
            {
                case "CSharpCodeMatcher":
                    if (_settings.AllowCSharpCodeMatcher == true)
                    {
                        return new CSharpCodeMatcher(matchBehaviour, stringPatterns);
                    }

                    throw new NotSupportedException("It's not allowed to use the 'CSharpCodeMatcher' because IWireMockServerSettings.AllowCSharpCodeMatcher is not set to 'true'.");

                case "LinqMatcher":
                    return new LinqMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case "ExactMatcher":
                    return new ExactMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case "ExactObjectMatcher":
                    return CreateExactObjectMatcher(matchBehaviour, stringPatterns[0], throwExceptionWhenMatcherFails);

                case "RegexMatcher":
                    return new RegexMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case "JsonMatcher":
                    return new JsonMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case "JsonPathMatcher":
                    return new JsonPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case "JmesPathMatcher":
                    return new JmesPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case "XPathMatcher":
                    return new XPathMatcher(matchBehaviour, throwExceptionWhenMatcherFails, stringPatterns);

                case "WildcardMatcher":
                    return new WildcardMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case "ContentTypeMatcher":
                    return new ContentTypeMatcher(matchBehaviour, stringPatterns, ignoreCase, throwExceptionWhenMatcherFails);

                case "SimMetricsMatcher":
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

        private ExactObjectMatcher CreateExactObjectMatcher(MatchBehaviour matchBehaviour, string stringPattern, bool throwException)
        {
            byte[] bytePattern;
            try
            {
                bytePattern = Convert.FromBase64String(stringPattern);
            }
            catch
            {
                throw new ArgumentException($"Matcher 'ExactObjectMatcher' has invalid pattern. The pattern value '{stringPattern}' is not a Base64String.", nameof(stringPattern));
            }

            return new ExactObjectMatcher(matchBehaviour, bytePattern, throwException);
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

            object[] patterns = new object[0]; // Default empty array
            switch (matcher)
            {
                // If the matcher is a IStringMatcher, get the patterns.
                case IStringMatcher stringMatcher:
                    patterns = stringMatcher.GetPatterns().Cast<object>().ToArray();
                    break;

                // If the matcher is a IValueMatcher, get the value (can be string or object).
                case IValueMatcher valueMatcher:
                    patterns = new[] { valueMatcher.Value };
                    break;

                // If the matcher is a ExactObjectMatcher, get the ValueAsObject or ValueAsBytes.
                case ExactObjectMatcher exactObjectMatcher:
                    patterns = new[] { exactObjectMatcher.ValueAsObject ?? exactObjectMatcher.ValueAsBytes };
                    break;
            }

            bool? ignoreCase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : (bool?)null;

            bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : (bool?)null;

            return new MatcherModel
            {
                RejectOnMatch = rejectOnMatch,
                IgnoreCase = ignoreCase,
                Name = matcher.Name,
                Pattern = patterns.Length == 1 ? patterns.First() : null,
                Patterns = patterns.Length > 1 ? patterns : null
            };
        }
    }
}