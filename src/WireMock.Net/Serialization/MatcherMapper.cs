using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using SimMetrics.Net;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;
using WireMock.Plugin;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Serialization;

internal class MatcherMapper
{
    private readonly WireMockServerSettings _settings;

    public MatcherMapper(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);
    }

    public IMatcher[]? Map(IEnumerable<MatcherModel>? matchers)
    {
        if (matchers == null)
        {
            return null;
        }
        return matchers.Select(Map).Where(m => m != null).ToArray()!;
    }

    public IMatcher? Map(MatcherModel? matcher)
    {
        if (matcher == null)
        {
            return null;
        }

        string[] parts = matcher.Name.Split('.');
        string matcherName = parts[0];
        string? matcherType = parts.Length > 1 ? parts[1] : null;
        var stringPatterns = ParseStringPatterns(matcher);
        var matchBehaviour = matcher.RejectOnMatch == true ? MatchBehaviour.RejectOnMatch : MatchBehaviour.AcceptOnMatch;
        var matchOperator = StringUtils.ParseMatchOperator(matcher.MatchOperator);
        bool ignoreCase = matcher.IgnoreCase == true;
        bool useRegexExtended = _settings.UseRegexExtended == true;
        bool useRegex = matcher.Regex == true;

        switch (matcherName)
        {
            case nameof(NotNullOrEmptyMatcher):
                return new NotNullOrEmptyMatcher(matchBehaviour);

            case "CSharpCodeMatcher":
                if (_settings.AllowCSharpCodeMatcher == true)
                {
                    return PluginLoader.Load<ICSharpCodeMatcher>(matchBehaviour, matchOperator, stringPatterns);
                }

                throw new NotSupportedException("It's not allowed to use the 'CSharpCodeMatcher' because WireMockServerSettings.AllowCSharpCodeMatcher is not set to 'true'.");

            case nameof(LinqMatcher):
                return new LinqMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(ExactMatcher):
                return new ExactMatcher(matchBehaviour, ignoreCase, matchOperator, stringPatterns);

            case nameof(ExactObjectMatcher):
                return CreateExactObjectMatcher(matchBehaviour, stringPatterns[0]);
#if GRAPHQL
            case nameof(GraphQLMatcher):
                return new GraphQLMatcher(stringPatterns[0].GetPattern(), matcher.CustomScalars, matchBehaviour, matchOperator);
#endif

#if MIMEKIT
            case nameof(MimePartMatcher):
                return CreateMimePartMatcher(matchBehaviour, matcher);
#endif
            case nameof(RegexMatcher):
                return new RegexMatcher(matchBehaviour, stringPatterns, ignoreCase, useRegexExtended, matchOperator);

            case nameof(JsonMatcher):
                var valueForJsonMatcher = matcher.Pattern ?? matcher.Patterns;
                return new JsonMatcher(matchBehaviour, valueForJsonMatcher!, ignoreCase);

            case nameof(JsonPartialMatcher):
                var valueForJsonPartialMatcher = matcher.Pattern ?? matcher.Patterns;
                return new JsonPartialMatcher(matchBehaviour, valueForJsonPartialMatcher!, ignoreCase, useRegex);

            case nameof(JsonPartialWildcardMatcher):
                var valueForJsonPartialWildcardMatcher = matcher.Pattern ?? matcher.Patterns;
                return new JsonPartialWildcardMatcher(matchBehaviour, valueForJsonPartialWildcardMatcher!, ignoreCase, useRegex);

            case nameof(JsonPathMatcher):
                return new JsonPathMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(JmesPathMatcher):
                return new JmesPathMatcher(matchBehaviour, matchOperator, stringPatterns);

            case nameof(XPathMatcher):
                return new XPathMatcher(matchBehaviour, matchOperator, matcher.XmlNamespaceMap, stringPatterns);

            case nameof(WildcardMatcher):
                return new WildcardMatcher(matchBehaviour, stringPatterns, ignoreCase, matchOperator);

            case nameof(ContentTypeMatcher):
                return new ContentTypeMatcher(matchBehaviour, stringPatterns, ignoreCase);

            case nameof(SimMetricsMatcher):
                SimMetricType type = SimMetricType.Levenstein;
                if (!string.IsNullOrEmpty(matcherType) && !Enum.TryParse(matcherType, out type))
                {
                    throw new NotSupportedException($"Matcher '{matcherName}' with Type '{matcherType}' is not supported.");
                }

                return new SimMetricsMatcher(matchBehaviour, stringPatterns, type);

            default:
                if (_settings.CustomMatcherMappings != null && _settings.CustomMatcherMappings.ContainsKey(matcherName))
                {
                    return _settings.CustomMatcherMappings[matcherName](matcher);
                }

                throw new NotSupportedException($"Matcher '{matcherName}' is not supported.");
        }
    }

    public MatcherModel[]? Map(IEnumerable<IMatcher>? matchers)
    {
        return matchers?.Where(m => m != null).Select(Map).ToArray();
    }

    public MatcherModel? Map(IMatcher? matcher)
    {
        if (matcher == null)
        {
            return null;
        }

        bool? ignoreCase = matcher is IIgnoreCaseMatcher ignoreCaseMatcher ? ignoreCaseMatcher.IgnoreCase : null;
        bool? rejectOnMatch = matcher.MatchBehaviour == MatchBehaviour.RejectOnMatch ? true : null;

        var model = new MatcherModel
        {
            RejectOnMatch = rejectOnMatch,
            IgnoreCase = ignoreCase,
            Name = matcher.Name
        };

        switch (matcher)
        {
            case JsonPartialMatcher jsonPartialMatcher:
                model.Regex = jsonPartialMatcher.Regex;
                break;

            case JsonPartialWildcardMatcher jsonPartialWildcardMatcher:
                model.Regex = jsonPartialWildcardMatcher.Regex;
                break;

            case XPathMatcher xpathMatcher:
                model.XmlNamespaceMap = xpathMatcher.XmlNamespaceMap;
                break;
#if GRAPHQL
            case GraphQLMatcher graphQLMatcher:
                model.CustomScalars = graphQLMatcher.CustomScalars;
                break;
#endif
        }

        switch (matcher)
        {
            // If the matcher is a IStringMatcher, get the operator & patterns.
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
                    model.MatchOperator = stringMatcher.MatchOperator.ToString();
                }
                break;

            // If the matcher is a IValueMatcher, get the value (can be string or object).
            case IValueMatcher valueMatcher:
                model.Pattern = valueMatcher.Value;
                break;

            // If the matcher is a ExactObjectMatcher, get the ValueAsObject or ValueAsBytes.
            case ExactObjectMatcher exactObjectMatcher:
                model.Pattern = exactObjectMatcher.ValueAsObject ?? exactObjectMatcher.ValueAsBytes;
                break;

#if MIMEKIT
            case MimePartMatcher mimePartMatcher:
                model.ContentDispositionMatcher = Map(mimePartMatcher.ContentDispositionMatcher);
                model.ContentMatcher = Map(mimePartMatcher.ContentMatcher);
                model.ContentTransferEncodingMatcher = Map(mimePartMatcher.ContentTransferEncodingMatcher);
                model.ContentTypeMatcher = Map(mimePartMatcher.ContentTypeMatcher);
                break;
#endif
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

        if (matcher.Patterns?.OfType<string>() is { } patternsAsStringArray)
        {
            return patternsAsStringArray.ToAnyOfPatterns();
        }

        if (!string.IsNullOrEmpty(matcher.PatternAsFile))
        {
            var patternAsFile = matcher.PatternAsFile!;
            var pattern = _settings.FileSystemHandler.ReadFileAsString(patternAsFile);
            return new[] { new AnyOf<string, StringPattern>(new StringPattern { Pattern = pattern, PatternAsFile = patternAsFile }) };
        }

        return EmptyArray<AnyOf<string, StringPattern>>.Value;
    }

    private static ExactObjectMatcher CreateExactObjectMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> stringPattern)
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

        return new ExactObjectMatcher(matchBehaviour, bytePattern);
    }

#if MIMEKIT
    private MimePartMatcher CreateMimePartMatcher(MatchBehaviour matchBehaviour, MatcherModel? matcher)
    {
        var contentTypeMatcher = Map(matcher?.ContentTypeMatcher) as IStringMatcher;
        var contentDispositionMatcher = Map(matcher?.ContentDispositionMatcher) as IStringMatcher;
        var contentTransferEncodingMatcher = Map(matcher?.ContentTransferEncodingMatcher) as IStringMatcher;
        var contentMatcher = Map(matcher?.ContentMatcher);

        return new MimePartMatcher(matchBehaviour, contentTypeMatcher, contentDispositionMatcher, contentTransferEncodingMatcher, contentMatcher);
    }
#endif
}