using Stef.Validation;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Matchers.Helpers;

internal static class BodyDataMatchScoreCalculator
{
    public static double CalculateMatchScore(IBodyData? requestMessage, IMatcher matcher)
    {
        Guard.NotNull(matcher);

        if (matcher is NotNullOrEmptyMatcher notNullOrEmptyMatcher)
        {
            switch (requestMessage?.DetectedBodyType)
            {
                case BodyType.Json:
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyAsString).Score;

                case BodyType.Bytes:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyAsBytes).Score;

                default:
                    return MatchScores.Mismatch;
            }
        }

        if (matcher is ExactObjectMatcher exactObjectMatcher)
        {
            // If the body is a byte array, try to match.
            var detectedBodyType = requestMessage?.DetectedBodyType;
            if (detectedBodyType is BodyType.Bytes or BodyType.String or BodyType.FormUrlEncoded)
            {
                return exactObjectMatcher.IsMatch(requestMessage?.BodyAsBytes).Score;
            }
        }

        // Check if the matcher is a IObjectMatcher
        if (matcher is IObjectMatcher objectMatcher)
        {
            // If the body is a JSON object, try to match.
            if (requestMessage?.DetectedBodyType == BodyType.Json)
            {
                return objectMatcher.IsMatch(requestMessage.BodyAsJson).Score;
            }

            // If the body is a byte array, try to match.
            if (requestMessage?.DetectedBodyType == BodyType.Bytes)
            {
                return objectMatcher.IsMatch(requestMessage.BodyAsBytes).Score;
            }
        }

        // Check if the matcher is a IStringMatcher
        if (matcher is IStringMatcher stringMatcher)
        {
            // If the body is a Json or a String, use the BodyAsString to match on.
            if (requestMessage?.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
            {
                return stringMatcher.IsMatch(requestMessage.BodyAsString).Score;
            }
        }

#if MIMEKIT_XXX
        if (matcher is MultiPartMatcher multiPartMatcher)
        {
            // If the body is a String or MultiPart, use the BodyAsString to match on.
            if (requestMessage?.DetectedBodyType is BodyType.String or BodyType.MultiPart)
            {
                return multiPartMatcher.IsMatch(requestMessage.BodyAsString);
            }
        }
#endif
        return MatchScores.Mismatch;
    }
}