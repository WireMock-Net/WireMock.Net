using Stef.Validation;

namespace WireMock.Matchers;

/// <summary>
/// The MatchResult which contains the score (value between 0.0 - 1.0 of the similarity) and an optional error message.
/// </summary>
public struct MatchResult
{
    /// <summary>
    /// A value between 0.0 - 1.0 of the similarity.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// The error message (exception message) in case the  matching fails.
    /// [Optional]
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Create a MatchResult
    /// </summary>
    /// <param name="score">A value between 0.0 - 1.0 of the similarity.</param>
    /// <param name="error">The error message (exception) in case the  matching fails. [Optional]</param>
    public MatchResult(double score, string? error = null)
    {
        Score = score;
        Error = error;
    }

    /// <summary>
    /// Create a MatchResult
    /// </summary>
    /// <param name="error">The error message (exception message) in case the  matching fails.</param>
    public MatchResult(string error)
    {
        Error = Guard.NotNull(error);
    }

    /// <summary>
    /// Implicitly converts a double to a MatchResult.
    /// </summary>
    /// <param name="score">The score</param>
    public static implicit operator MatchResult(double score)
    {
        return new MatchResult(score);
    }

    /// <summary>
    /// Is the value a perfect match?
    /// </summary>
    public bool IsPerfect() => MatchScores.IsPerfect(Score);
}