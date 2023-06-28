#if !(NET451 || NET452 || NET46 || NET461 || NETSTANDARD1_3)
using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using HotChocolate;
using HotChocolate.Execution;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// GrapQLMatcher Schema Matcher
/// </summary>
/// <inheritdoc cref="IStringMatcher"/>
public class GraphQLMatcher : IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    private readonly ISchema _schema;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="schema">The schema.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    public GraphQLMatcher(AnyOf<string, StringPattern, ISchema> schema, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, bool throwException = false, MatchOperator matchOperator = MatchOperator.Or)
    {
        Guard.NotNull(schema);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
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
                _schema = BuildSchema(schema.Second.Pattern); // TODO : check if that is correct
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
    public double IsMatch(string? input)
    {
        var match = MatchScores.Mismatch;

        try
        {
            if (_schema.MakeExecutable().ExecuteAsync(input!).GetAwaiter().GetResult() is not QueryResult queryResult)
            {
                throw new GraphQLException("Invalid GraphQL execution result.");
            }

            if (queryResult.Errors == null || queryResult.Errors.Count == 0)
            {
                match = MatchScores.Perfect;
            }
            else
            {
                var exceptions = queryResult.Errors.Select(e => e.Exception ?? new GraphQLException(e.Message)).ToArray();
                if (exceptions.Length == 1)
                {
                    throw exceptions[0];
                }

                throw new AggregateException(exceptions);
            }
        }
        catch
        {
            if (ThrowException)
            {
                throw;
            }
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, match);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => nameof(GraphQLMatcher);

    private static ISchema BuildSchema(string schema)
    {
        return SchemaBuilder.New()
            .AddDocumentFromString(schema)
            .Use(next => next.Invoke)
            .Create();
    }
}
#endif