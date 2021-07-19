﻿namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// ClientIPModel
    /// </summary>
#if !STEF_NET45
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class ClientIPModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}