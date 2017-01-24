using System.Collections.Generic;

namespace WireMock.Admin
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
        public IList<MatcherModel> Matchers { get; set; }
    }
}