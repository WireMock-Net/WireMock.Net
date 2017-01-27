using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// UrlModel
    /// </summary>
    public class UrlModel
    {
        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        /// <value>
        /// The matchers.
        /// </value>
        public MatcherModel[] Matchers { get; set; }
    }
}