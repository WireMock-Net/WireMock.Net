namespace WireMock.RequestBuilders
{
    /// <summary>
    /// The ParametersRequestBuilder interface.
    /// </summary>
    public interface IParamsRequestBuilder
    {
        /// <summary>
        /// The with parameters.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="ISpecifyRequests"/>.
        /// </returns>
        ISpecifyRequests WithParam(string key, params string[] values);
    }
}