using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// System.Linq.Dynamic.Core Expression Matcher
/// </summary>
/// <inheritdoc cref="IObjectMatcher"/>
/// <inheritdoc cref="IStringMatcher"/>
public class LinqMatcher : IObjectMatcher, IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IMatcher.ThrowException"/>
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    public LinqMatcher(AnyOf<string, StringPattern> pattern) : this(new[] { pattern })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public LinqMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    public LinqMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> pattern) : this(matchBehaviour, false, MatchOperator.Or, pattern)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinqMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public LinqMatcher(
        MatchBehaviour matchBehaviour,
        bool throwException = false,
        MatchOperator matchOperator = MatchOperator.Or,
        params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc cref="IStringMatcher.IsMatch"/>
    public double IsMatch(string? input)
    {
        double match = MatchScores.Mismatch;

        // Convert a single input string to a Queryable string-list with 1 entry.
        IQueryable queryable = new[] { input }.AsQueryable();

        try
        {
            // Use the Any(...) method to check if the result matches
            match = MatchScores.ToScore(_patterns.Select(pattern => queryable.Any(pattern.GetPattern())).ToArray(), MatchOperator);

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
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

    /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
    public double IsMatch(object? input)
    {
        double match = MatchScores.Mismatch;

        JObject value;
        switch (input)
        {
            case JObject valueAsJObject:
                value = valueAsJObject;
                break;

            case { } valueAsObject:
                value = JObject.FromObject(valueAsObject);
                break;

            default:
                return MatchScores.Mismatch;
        }

        // Convert a single object to a Queryable JObject-list with 1 entry.
        var queryable1 = new[] { value }.AsQueryable();

        try
        {
            // Generate the DynamicLinq select statement.
            string dynamicSelect = JsonUtils.GenerateDynamicLinqStatement(value);

            // Execute DynamicLinq Select statement.
            var queryable2 = queryable1.Select(dynamicSelect);

            // Use the Any(...) method to check if the result matches.
            match = MatchScores.ToScore(_patterns.Select(pattern => queryable2.Any(pattern)).ToArray(), MatchOperator);

            return MatchBehaviourHelper.Convert(MatchBehaviour, match);
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

    /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "LinqMatcher";
}