#if GRAPHQL
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnyOfTypes;
using GraphQL;
using GraphQL.Types;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// GrapQLMatcher Schema Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
public class GraphQLMatcher : IStringMatcher
{
    private sealed class GraphQLRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string? Query { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public Dictionary<string, object?>? Variables { get; set; }
    }

    private readonly AnyOf<string, StringPattern>[] _patterns;

    private readonly ISchema _schema;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public GraphQLMatcher(AnyOf<string, StringPattern, ISchema> schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, MatchOperator matchOperator = MatchOperator.Or)
    {
        Guard.NotNull(schema);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;

        var patterns = new List<AnyOf<string, StringPattern>>();
        switch (schema.CurrentType)
        {
            case AnyOfType.First:
                patterns.Add(schema.First);
                _schema = BuildSchema(schema);
                break;

            case AnyOfType.Second:
                patterns.Add(schema.Second);
                _schema = BuildSchema(schema.Second.Pattern);
                break;

            case AnyOfType.Third:
                _schema = schema.Third;
                break;

            default:
                throw new NotSupportedException();
        }
        _patterns = patterns.ToArray();
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (input != null && TryGetGraphQLRequest(input, out var graphQLRequest))
        {
            try
            {
                var executionResult = new DocumentExecuter().ExecuteAsync(_ =>
                {
                    _.ThrowOnUnhandledException = true;

                    _.Schema = _schema;
                    _.Query = graphQLRequest.Query;

                    if (graphQLRequest.Variables != null)
                    {
                        _.Variables = new Inputs(graphQLRequest.Variables);
                    }
                }).GetAwaiter().GetResult();

                if (executionResult.Errors == null || executionResult.Errors.Count == 0)
                {
                    score = MatchScores.Perfect;
                }
                else
                {
                    exception = executionResult.Errors.OfType<Exception>().ToArray().ToException();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(GraphQLMatcher);

    private static bool TryGetGraphQLRequest(string input, [NotNullWhen(true)] out GraphQLRequest? graphQLRequest)
    {
        try
        {
            graphQLRequest = JsonConvert.DeserializeObject<GraphQLRequest>(input);
            return graphQLRequest != null;
        }
        catch
        {
            graphQLRequest = default;
            return false;
        }
    }

    private static ISchema BuildSchema(string schema)
    {
        return Schema.For(schema);
    }
}
#endif