// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using Stef.Validation;
using WireMock.Extensions;

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
    /// The exception message) in case the matching fails.
    /// [Optional]
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Create a MatchResult
    /// </summary>
    /// <param name="score">A value between 0.0 - 1.0 of the similarity.</param>
    /// <param name="exception">The exception in case the matching fails. [Optional]</param>
    public MatchResult(double score, Exception? exception = null)
    {
        Score = score;
        Exception = exception;
    }

    /// <summary>
    /// Create a MatchResult
    /// </summary>
    /// <param name="exception">The exception in case the matching fails.</param>
    public MatchResult(Exception exception)
    {
        Exception = Guard.NotNull(exception);
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

    /// <summary>
    /// Create a MatchResult from multiple MatchResults.
    /// </summary>
    /// <param name="matchResults">A list of MatchResults.</param>
    /// <param name="matchOperator">The MatchOperator</param>
    /// <returns>MatchResult</returns>
    public static MatchResult From(IReadOnlyList<MatchResult> matchResults, MatchOperator matchOperator)
    {
        Guard.NotNullOrEmpty(matchResults);

        if (matchResults.Count == 1)
        {
            return matchResults[0];
        }

        return new MatchResult
        {
            Score = MatchScores.ToScore(matchResults.Select(r => r.Score).ToArray(), matchOperator),
            Exception = matchResults.Select(m => m.Exception).OfType<Exception>().ToArray().ToException()
        };
    }

    /// <summary>
    /// Expand to Tuple
    /// </summary>
    /// <returns>Tuple : Score and Exception</returns>
    public (double Score, Exception? Exception) Expand()
    {
        return (Score, Exception);
    }
}