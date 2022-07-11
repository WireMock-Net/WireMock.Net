namespace WireMock.Matchers;

/// <summary>
/// JsonPartialWildCardMatcher
/// </summary>
public class JsonPartialWildcardMatcher : AbstractJsonPartialMatcher
{
    /// <inheritdoc />
    public override string Name => nameof(JsonPartialWildcardMatcher);

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(string value, bool ignoreCase = false, bool throwException = false, bool regex = false)
        : base(value, ignoreCase, throwException, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(object value, bool ignoreCase = false, bool throwException = false, bool regex = false)
        : base(value, ignoreCase, throwException, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool throwException = false, bool regex = false)
        : base(matchBehaviour, value, ignoreCase, throwException, regex)
    {
    }

    /// <inheritdoc />
    protected override bool IsMatch(string value, string input)
    {
        var wildcardStringMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, value, IgnoreCase);
        return MatchScores.IsPerfect(wildcardStringMatcher.IsMatch(input));
    }
}