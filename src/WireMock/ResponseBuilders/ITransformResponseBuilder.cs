namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The BodyResponseBuilder interface.
    /// </summary>
    public interface ITransformResponseBuilder : IDelayResponseBuilder
    {
        /// <summary>
        /// The with transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IDelayResponseBuilder"/>.
        /// </returns>
        IDelayResponseBuilder WithTransformer();
    }
}