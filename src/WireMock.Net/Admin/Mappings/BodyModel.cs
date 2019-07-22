﻿namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Body Model
    /// </summary>
    public class BodyModel
    {
        /// <summary>
        /// Gets or sets the matcher.
        /// </summary>
        public MatcherModel Matcher { get; set; }

        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}