using System.Linq;
using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// ExactObjectMatcher
/// </summary>
/// <seealso cref="IObjectMatcher" />
public class ExactObjectMatcher : IObjectMatcher
{
    /// <summary>
    /// Gets the value as object.
    /// </summary>
    public object? ValueAsObject { get; }

    /// <summary>
    /// Gets the value as byte[].
    /// </summary>
    public byte[]? ValueAsBytes { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(object value) : this(MatchBehaviour.AcceptOnMatch, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(MatchBehaviour matchBehaviour, object value)
    {
        ValueAsObject = Guard.NotNull(value);
        MatchBehaviour = matchBehaviour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(byte[] value) : this(MatchBehaviour.AcceptOnMatch, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExactObjectMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="value">The value.</param>
    public ExactObjectMatcher(MatchBehaviour matchBehaviour, byte[] value, bool throwException = false)
    {
        ValueAsBytes = Guard.NotNull(value);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
    }

    /// <inheritdoc cref="IObjectMatcher.IsMatch"/>
    public double IsMatch(object? input)
    {
        bool equals = false;
        if (ValueAsObject != null)
        {
            equals = Equals(ValueAsObject, input);
        }
        else if (input != null)
        {
            equals = ValueAsBytes?.SequenceEqual((byte[])input) == true;
        }

        return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(equals));
    }

    /// <inheritdoc cref="IMatcher.Name"/>
    public string Name => "ExactObjectMatcher";
}