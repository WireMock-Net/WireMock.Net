﻿namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// UrlModel
    /// </summary>
#if !NET45
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class UrlModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}
