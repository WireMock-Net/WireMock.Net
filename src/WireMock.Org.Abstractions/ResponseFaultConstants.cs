// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions
{
    /// <summary>
    /// The fault to apply (instead of a full, valid response).
    /// </summary>
    public static class ResponseFaultConstants
    {
        public const string CONNECTIONRESETBYPEER = "CONNECTION_RESET_BY_PEER";

        public const string EMPTYRESPONSE = "EMPTY_RESPONSE";

        public const string MALFORMEDRESPONSECHUNK = "MALFORMED_RESPONSE_CHUNK";

        public const string RANDOMDATATHENCLOSE = "RANDOM_DATA_THEN_CLOSE";
    }
}
