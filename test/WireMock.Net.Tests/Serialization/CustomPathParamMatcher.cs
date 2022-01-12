using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AnyOfTypes;
using Newtonsoft.Json;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Net.Tests.Serialization
{
    /// <summary>
    /// This matcher is only for unit test purposes
    /// </summary>
    public class CustomPathParamMatcher : IStringMatcher
    {
        public string Name => nameof(CustomPathParamMatcher);
        public MatchBehaviour MatchBehaviour { get; }
        public bool ThrowException { get; }

        private readonly string _path;
        private readonly string[] _pathParts;
        private readonly Dictionary<string, string> _pathParams;

        public CustomPathParamMatcher(string path, Dictionary<string, string> pathParams) : this(MatchBehaviour.AcceptOnMatch, path, pathParams)
        {

        }

        public CustomPathParamMatcher(MatchBehaviour matchBehaviour, string path, Dictionary<string, string> pathParams, bool throwException = false)
        {
            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;
            _path = path;
            _pathParts = GetPathParts(path);
            _pathParams = pathParams.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public double IsMatch(string input)
        {
            var inputParts = GetPathParts(input);
            if (inputParts.Length != _pathParts.Length)
                return MatchScores.Mismatch;

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
                            return MatchScores.Mismatch;

                        if (!Regex.IsMatch(inputPart, _pathParams[pathParamName], RegexOptions.IgnoreCase))
                            return MatchScores.Mismatch;
                    }
                    else
                    {
                        if (!inputPart.Equals(pathPart, StringComparison.InvariantCultureIgnoreCase))
                            return MatchScores.Mismatch;
                    }
                }
            }
            catch
            {
                if (ThrowException)
                    throw;
            }

            return MatchScores.Perfect;
        }

        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return new[] { new AnyOf<string, StringPattern>(JsonConvert.SerializeObject(new CustomPathParamMatcherModel(_path, _pathParams))) };
            //return _pathParams.Values.Select(x => new AnyOf<string, StringPattern>(x)).ToArray();
        }

        private string[] GetPathParts(string path)
        {
            var hashMarkIndex = path.IndexOf('#');
            if (hashMarkIndex != -1)
                path = path.Substring(0, hashMarkIndex);

            var queryParamsIndex = path.IndexOf('?');
            if (queryParamsIndex != -1)
                path = path.Substring(0, queryParamsIndex);

            return path.Trim().Trim('/').ToLower().Split('/');
        }
    }

    public class CustomPathParamMatcherModel
    {
        public string Path { get; set; }
        public Dictionary<string, string> PathParams { get; set; }

        public CustomPathParamMatcherModel()
        {
            
        }

        public CustomPathParamMatcherModel(string path, Dictionary<string, string> pathParams)
        {
            Path = path;
            PathParams = pathParams;
        }
    }
}
