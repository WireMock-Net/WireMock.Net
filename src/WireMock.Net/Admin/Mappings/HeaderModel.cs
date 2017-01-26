using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Header Model
    /// </summary>
    public class HeaderModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the matchers.
        /// </summary>
        /// <value>
        /// The matchers.
        /// </value>
        public IList<MatcherModel> Matchers { get; set; }
    }
}