using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Matchers.Request;

/// <summary>
/// RequestMatchResult
/// </summary>
public class RequestMatchResult : IRequestMatchResult
{
    /// <inheritdoc cref="IRequestMatchResult.TotalScore" />
    public double TotalScore => MatchDetails.Sum(md => md.Score);

    /// <inheritdoc cref="IRequestMatchResult.TotalNumber" />
    public int TotalNumber => MatchDetails.Count;

    /// <inheritdoc cref="IRequestMatchResult.IsPerfectMatch" />
    public bool IsPerfectMatch => Math.Abs(TotalScore - TotalNumber) < MatchScores.Tolerance;

    /// <inheritdoc cref="IRequestMatchResult.AverageTotalScore" />
    public double AverageTotalScore => TotalNumber == 0 ? 0.0 : TotalScore / TotalNumber;

    /// <inheritdoc cref="IRequestMatchResult.MatchDetails" />
    public IList<MatchDetail> MatchDetails { get; } = new List<MatchDetail>();

    /// <summary>
    /// Adds the score.
    /// </summary>
    /// <param name="matcherType">The matcher Type.</param>
    /// <param name="score">The score.</param>
    /// <param name="error">The error (exception).</param>
    /// <returns>The score.</returns>
    public double AddScore(Type matcherType, double score, string? error)
    {
        MatchDetails.Add(new MatchDetail { MatcherType = matcherType, Score = score, Error = error});

        return score;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    public int CompareTo(object obj)
    {
        var compareObj = (RequestMatchResult)obj;

        int averageTotalScoreResult = compareObj.AverageTotalScore.CompareTo(AverageTotalScore);

        // In case the score is equal, prefer the one with the most matchers.
        return averageTotalScoreResult == 0 ? compareObj.TotalNumber.CompareTo(TotalNumber) : averageTotalScoreResult;
    }
}