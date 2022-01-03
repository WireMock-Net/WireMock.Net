using WireMock.Types;

namespace WireMock.ResponseBuilders
{
    /// <summary>
    /// The TransformResponseBuilder interface.
    /// </summary>
    public interface ITransformResponseBuilder : IDelayResponseBuilder
    {
        /// <summary>
        /// Use the Handlebars.Net ResponseMessage transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithTransformer(bool transformContentFromBodyAsFile);

        /// <summary>
        /// Use the Handlebars.Net ResponseMessage transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithTransformer(ReplaceNodeOption option);

        /// <summary>
        /// Use a specific ResponseMessage transformer.
        /// </summary>
        /// <returns>
        /// The <see cref="IResponseBuilder"/>.
        /// </returns>
        IResponseBuilder WithTransformer(TransformerType transformerType = TransformerType.Handlebars, bool transformContentFromBodyAsFile = false, ReplaceNodeOption option = ReplaceNodeOption.Bool);
    }
}