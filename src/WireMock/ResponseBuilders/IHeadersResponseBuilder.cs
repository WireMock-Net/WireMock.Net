namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The HeadersResponseBuilder interface.
    /// </summary>
    public interface IHeadersResponseBuilder : IBodyResponseBuilder
    {
        /// <summary>
        /// The with header.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithHeader(string name, string value);
    }
}