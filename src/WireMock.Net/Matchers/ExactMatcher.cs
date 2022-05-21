using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// ExactMatcher
/// </summary>
/// <seealso cref="IStringMatcher" />
public class ExactMatcher : IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _values;

    /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IMatcher.ThrowException"/>
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    public ExactMatcher(params AnyOf<string, StringPattern>[] values) : this(MatchBehaviour.AcceptOnMatch, false, MatchOperator.Or, values)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="values">The values.</param>
    public ExactMatcher(
        MatchBehaviour matchBehaviour,
        bool throwException = false,
        MatchOperator matchOperator = MatchOperator.Or,
        params AnyOf<string, StringPattern>[] values)
    {
        _values = Guard.NotNull(values);

        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc cref="IStringMatcher.IsMatch"/>
    public double IsMatch(string input)
    {
        double score = MatchScores.ToScore(_values.Select(v => v.GetPattern() == input).ToArray(), MatchOperator);
        return MatchBehaviourHelper.Convert(MatchBehaviour, score);
    }

    /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _values;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "ExactMatcher";
}