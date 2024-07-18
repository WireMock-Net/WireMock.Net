// Copyright © WireMock.Net

namespace WireMock.Types;

/// <summary>
/// The ResponseMessage Transformers
/// </summary>
public enum TransformerType
{
    /// <summary>
    /// https://github.com/Handlebars-Net/Handlebars.Net
    /// </summary>
    Handlebars,

    /// <summary>
    /// https://github.com/scriban/scriban : default
    /// </summary>
    Scriban,

    /// <summary>
    /// https://github.com/scriban/scriban : DotLiquid
    /// </summary>
    ScribanDotLiquid
}