namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The TransformResponseBuilder interface.
    /// </summary>
    public interface ITransformResponseBuilder : IDelayResponseBuilder
    {
        /// <summary>
        /// The with transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithTransformer();
    }
}