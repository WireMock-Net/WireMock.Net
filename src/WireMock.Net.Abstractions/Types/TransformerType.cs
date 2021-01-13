﻿namespace WireMock.Types
{
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
        /// https://github.com/dotliquid/dotliquid
        /// </summary>
        DotLiquid

        ///// <summary>
        ///// https://github.com/scriban/scriban
        ///// </summary>
        //Scriban,

        ///// <summary>
        ///// https://github.com/scriban/scriban (Liquid)
        ///// </summary>
        //ScribanLiquid
    }
}