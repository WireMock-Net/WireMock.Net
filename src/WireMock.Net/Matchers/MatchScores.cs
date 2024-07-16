// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Matchers;

/// <summary>
/// MatchScores
/// </summary>
public static class MatchScores
{
    /// <summary>
    /// The tolerance
    /// </summary>
    public const double Tolerance = 0.000001;

    /// <summary>
    /// The default mismatch score
    /// </summary>
    public const double Mismatch = 0.0;

    /// <summary>
    /// The default perfect match score
    /// </summary>
    public const double Perfect = 1.0;

    /// <summary>
    /// The almost perfect match score
    /// </summary>
    public const double AlmostPerfect = 0.99;

    /// <summary>
    /// Is the value a perfect match?
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true/false</returns>
    public static bool IsPerfect(double value)
    {
        return Math.Abs(value - Perfect) < Tolerance;
    }

    /// <summary>
    /// Convert a bool to the score.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>score</returns>
    public static double ToScore(bool value)
    {
        return value ? Perfect : Mismatch;
    }

    /// <summary>
    /// Calculates the score from multiple values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/>.</param>
    /// <returns>average score</returns>
    public static double ToScore(IReadOnlyCollection<bool> values, MatchOperator matchOperator)
    {
        return ToScore(values.Select(ToScore).ToArray(), matchOperator);
    }

    /// <summary>
    /// Calculates the score from multiple values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="matchOperator"></param>
    /// <returns>average score</returns>
    public static double ToScore(IReadOnlyCollection<double> values, MatchOperator matchOperator)
    {
        if (!values.Any())
        {
            return Mismatch;
        }

        return matchOperator switch
        {
            MatchOperator.Or => ToScore(values.Any(IsPerfect)),
            MatchOperator.And => ToScore(values.All(IsPerfect)),
            _ => values.Average()
        };
    }
}