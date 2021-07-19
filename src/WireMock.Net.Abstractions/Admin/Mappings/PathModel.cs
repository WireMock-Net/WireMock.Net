﻿namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// PathModel
    /// </summary>
#if !STEF_NET45
    [FluentBuilder.AutoGenerateBuilder]
#endif
    public class PathModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}