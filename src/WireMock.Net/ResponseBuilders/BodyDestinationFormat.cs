namespace WireMock.ResponseBuilders
{
    public static class BodyDestinationFormat
    {
        /// <summary>
        /// Same as source (no conversion)
        /// </summary>
        public const string SameAsSource = "SameAsSource";

        /// <summary>
        /// Convert to string
        /// </summary>
        public const string String = "String";

        /// <summary>
        /// Convert to bytes
        /// </summary>
        public const string Bytes = "Bytes";
    }
}