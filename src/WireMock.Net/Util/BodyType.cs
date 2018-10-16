namespace WireMock.Util
{
    /// <summary>
    /// The BodyType
    /// </summary>
    public enum BodyType
    {
        /// <summary>
        /// No body present
        /// </summary>
        None,

        /// <summary>
        /// Body is a String
        /// </summary>
        String,

        /// <summary>
        /// Body is a Json object
        /// </summary>
        Json,

        /// <summary>
        /// Body is a Byte array
        /// </summary>
        Bytes,

        /// <summary>
        /// Body is a File
        /// </summary>
        File
    }
}