using JetBrains.Annotations;
using SimMetrics.Net;
using SimMetrics.Net.Metric;
using WireMock.Validation;

namespace WireMock.Matchers
{
    /// <summary>
    /// SimMetricsMatcher
    /// </summary>
    /// <seealso cref="IMatcher" />
    public class SimMetricsMatcher : IMatcher
    {
        private readonly string _pattern;
        private readonly SimMetricType _simMetricType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimMetricsMatcher"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="simMetricType">The SimMetric Type</param>
        public SimMetricsMatcher([NotNull] string pattern, SimMetricType simMetricType = SimMetricType.Levenstein)
        {
            Check.NotNull(pattern, nameof(pattern));

            _pattern = pattern;
            _simMetricType = simMetricType;
        }

        /// <summary>
        /// Determines whether the specified input is match.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
        public double IsMatch(string input)
        {
            switch (_simMetricType)
            {
                case SimMetricType.BlockDistance:
                    var sim2 = new BlockDistance();
                    return sim2.GetSimilarity(_pattern, input);
                case SimMetricType.ChapmanLengthDeviation:
                    var sim3 = new ChapmanLengthDeviation();
                    return sim3.GetSimilarity(_pattern, input);
                case SimMetricType.CosineSimilarity:
                    var sim4 = new CosineSimilarity();
                    return sim4.GetSimilarity(_pattern, input);
                case SimMetricType.DiceSimilarity:
                    var sim5 = new DiceSimilarity();
                    return sim5.GetSimilarity(_pattern, input);
                case SimMetricType.EuclideanDistance:
                    var sim6 = new EuclideanDistance();
                    return sim6.GetSimilarity(_pattern, input);
                case SimMetricType.JaccardSimilarity:
                    var sim7 = new JaccardSimilarity();
                    return sim7.GetSimilarity(_pattern, input);
                case SimMetricType.Jaro:
                    var sim8 = new Jaro();
                    return sim8.GetSimilarity(_pattern, input);
                case SimMetricType.JaroWinkler:
                    var sim9 = new JaroWinkler();
                    return sim9.GetSimilarity(_pattern, input);
                case SimMetricType.MatchingCoefficient:
                    var sim10 = new MatchingCoefficient();
                    return sim10.GetSimilarity(_pattern, input);
                case SimMetricType.MongeElkan:
                    var sim11 = new MongeElkan();
                    return sim11.GetSimilarity(_pattern, input);
                case SimMetricType.NeedlemanWunch:
                    var sim12 = new NeedlemanWunch();
                    return sim12.GetSimilarity(_pattern, input);
                case SimMetricType.OverlapCoefficient:
                    var sim13 = new OverlapCoefficient();
                    return sim13.GetSimilarity(_pattern, input);
                case SimMetricType.QGramsDistance:
                    var sim14 = new QGramsDistance();
                    return sim14.GetSimilarity(_pattern, input);
                case SimMetricType.SmithWaterman:
                    var sim15 = new SmithWaterman();
                    return sim15.GetSimilarity(_pattern, input);
                case SimMetricType.SmithWatermanGotoh:
                    var sim16 = new SmithWatermanGotoh();
                    return sim16.GetSimilarity(_pattern, input);
                case SimMetricType.SmithWatermanGotohWindowedAffine:
                    var sim17 = new SmithWatermanGotohWindowedAffine();
                    return sim17.GetSimilarity(_pattern, input);
                case SimMetricType.ChapmanMeanLength:
                    var sim18 = new ChapmanMeanLength();
                    return sim18.GetSimilarity(_pattern, input);
                default:
                    var sim1 = new Levenstein();
                    return sim1.GetSimilarity(_pattern, input);
            }
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <returns>Pattern</returns>
        public string GetPattern()
        {
            return _pattern;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name</returns>
        public string GetName()
        {
            return $"SimMetricsMatcher ({_simMetricType})";
        }
    }
}