#if MIMEKIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnyOfTypes;
using GraphQL.Types;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// MultiPartMatcher
/// </summary>
public class MultiPartMatcher : IMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <inheritdoc />
    public string Name => nameof(MultiPartMatcher);

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    // public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public bool ThrowException { get; }

    /// <summary>
    /// 
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPartMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">IgnoreCase</param>
    /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public MultiPartMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false, bool throwException = false, MatchOperator matchOperator = MatchOperator.Or)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        ThrowException = throwException;
        //MatchOperator = matchOperator;
        ThrowException = throwException;
    }

    /// <inheritdoc />
    public double IsMatch(string? input)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }
}
#endif