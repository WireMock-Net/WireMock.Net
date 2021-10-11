using System.Linq;
using AnyOfTypes;
using JetBrains.Annotations;
using SimMetrics.Net;
using SimMetrics.Net.API;
using SimMetrics.Net.Metric;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// SimMetricsMatcher
    /// </summary>
    /// <seealso cref="IStringMatcher" />
    public class SimMetricsMatcher : IStringMatcher
    {
        private readonly AnyOf<string, StringPattern>[] _patterns;
        private readonly SimMetricType _simMetricType;

        /// <inheritdoc cref="IMatcher.MatchBehaviour"/>
        public MatchBehaviour MatchBehaviour { get; }

        /// <inheritdoc cref="IMatcher.ThrowException"/>
        public bool ThrowException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="simMetricType">The SimMetric Type</param>
        public SimMetricsMatcher([NotNull] AnyOf<string, StringPattern> pattern, SimMetricType simMetricType = SimMetricType.Levenstein) : this(new[] { pattern }, simMetricType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="simMetricType">The SimMetric Type</param>
        public SimMetricsMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern> pattern, SimMetricType simMetricType = SimMetricType.Levenstein) : this(matchBehaviour, new[] { pattern }, simMetricType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
        /// </summary>
        /// <param name="patterns">The patterns.</param>
        /// <param name="simMetricType">The SimMetric Type</param>
        public SimMetricsMatcher([NotNull] AnyOf<string, StringPattern>[] patterns, SimMetricType simMetricType = SimMetricType.Levenstein) : this(MatchBehaviour.AcceptOnMatch, patterns, simMetricType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
        /// </summary>
        /// <param name="matchBehaviour">The match behaviour.</param>
        /// <param name="patterns">The patterns.</param>
        /// <param name="simMetricType">The SimMetric Type</param>
        /// <param name="throwException">Throw an exception when the internal matching fails because of invalid input.</param>
        public SimMetricsMatcher(MatchBehaviour matchBehaviour, [NotNull] AnyOf<string, StringPattern>[] patterns, SimMetricType simMetricType = SimMetricType.Levenstein, bool throwException = false)
        {
            Check.NotNull(patterns, nameof(patterns));

            MatchBehaviour = matchBehaviour;
            ThrowException = throwException;

            _patterns = patterns;
            _simMetricType = simMetricType;
        }

        /// <inheritdoc cref="IStringMatcher.IsMatch"/>
        public double IsMatch(string input)
        {
            IStringMetric stringmetricType = GetStringMetricType();

            return MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(_patterns.Select(p => stringmetricType.GetSimilarity(p.GetPattern(), input))));
        }

        private IStringMetric GetStringMetricType()
        {
            switch (_simMetricType)
            {
                case SimMetricType.BlockDistance:
                    return new BlockDistance();
                case SimMetricType.ChapmanLengthDeviation:
                    return new ChapmanLengthDeviation();
                case SimMetricType.CosineSimilarity:
                    return new CosineSimilarity();
                case SimMetricType.DiceSimilarity:
                    return new DiceSimilarity();
                case SimMetricType.EuclideanDistance:
                    return new EuclideanDistance();
                case SimMetricType.JaccardSimilarity:
                    return new JaccardSimilarity();
                case SimMetricType.Jaro:
                    return new Jaro();
                case SimMetricType.JaroWinkler:
                    return new JaroWinkler();
                case SimMetricType.MatchingCoefficient:
                    return new MatchingCoefficient();
                case SimMetricType.MongeElkan:
                    return new MongeElkan();
                case SimMetricType.NeedlemanWunch:
                    return new NeedlemanWunch();
                case SimMetricType.OverlapCoefficient:
                    return new OverlapCoefficient();
                case SimMetricType.QGramsDistance:
                    return new QGramsDistance();
                case SimMetricType.SmithWaterman:
                    return new SmithWaterman();
                case SimMetricType.SmithWatermanGotoh:
                    return new SmithWatermanGotoh();
                case SimMetricType.SmithWatermanGotohWindowedAffine:
                    return new SmithWatermanGotohWindowedAffine();
                case SimMetricType.ChapmanMeanLength:
                    return new ChapmanMeanLength();
                default:
                    return new Levenstein();
            }
        }

        /// <inheritdoc cref="IStringMatcher.GetPatterns"/>
        public AnyOf<string, StringPattern>[] GetPatterns()
        {
            return _patterns;
        }

        /// <inheritdoc cref="IMatcher.Name"/>
        public string Name => $"SimMetricsMatcher.{_simMetricType}";
    }
}