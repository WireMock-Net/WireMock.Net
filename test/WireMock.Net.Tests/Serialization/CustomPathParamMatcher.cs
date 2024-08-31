// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Net.Tests.Serialization;

/// <summary>
/// This matcher is only for unit test purposes
/// </summary>
public class CustomPathParamMatcher : IStringMatcher
{
    public string Name => nameof(CustomPathParamMatcher);

    public MatchBehaviour MatchBehaviour { get; }

    private readonly string _path;
    private readonly string[] _pathParts;
    private readonly Dictionary<string, string> _pathParams;

    public CustomPathParamMatcher(string path, Dictionary<string, string> pathParams) : this(MatchBehaviour.AcceptOnMatch, path, pathParams)
    {
    }

    public CustomPathParamMatcher(
        MatchBehaviour matchBehaviour,
        string path,
        Dictionary<string, string> pathParams,
        MatchOperator matchOperator = MatchOperator.Or)
    {
        MatchBehaviour = matchBehaviour;
        _path = path;
        _pathParts = GetPathParts(path);
        _pathParams = pathParams.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        MatchOperator = matchOperator;
    }

    public MatchResult IsMatch(string? input)
    {
        var inputParts = GetPathParts(input);
        if (inputParts.Length != _pathParts.Length)
        {
            return MatchScores.Mismatch;
        }

        try
        {
            for (int i = 0; i < inputParts.Length; i++)
            {
                var inputPart = inputParts[i];
                var pathPart = _pathParts[i];
                if (pathPart.StartsWith("{") && pathPart.EndsWith("}"))
                {
                    var pathParamName = pathPart.Trim('{').Trim('}');
                    if (!_pathParams.ContainsKey(pathParamName))
                    {
                        return MatchScores.Mismatch;
                    }

                    if (!Regex.IsMatch(inputPart, _pathParams[pathParamName], RegexOptions.IgnoreCase))
                    {
                        return MatchScores.Mismatch;
                    }
                }
                else
                {
                    if (!inputPart.Equals(pathPart, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return MatchScores.Mismatch;
                    }
                }
            }
        }
        catch
        {
            return MatchScores.Mismatch;
        }

        return MatchScores.Perfect;
    }

    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return new[] { new AnyOf<string, StringPattern>(JsonConvert.SerializeObject(new CustomPathParamMatcherModel(_path, _pathParams))) };
    }

    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "// TODO: CustomPathParamMatcher";
    }

    private static string[] GetPathParts(string? path)
    {
        if (path is null)
        {
            return [];
        }

        var hashMarkIndex = path.IndexOf('#');
        if (hashMarkIndex != -1)
        {
            path = path.Substring(0, hashMarkIndex);
        }

        var queryParamsIndex = path.IndexOf('?');
        if (queryParamsIndex != -1)
        {
            path = path.Substring(0, queryParamsIndex);
        }

        return path.Trim().Trim('/').ToLower().Split('/');
    }
}