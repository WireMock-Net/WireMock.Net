using System.Collections.Generic;

namespace WireMock.Admin.Mappings
{
    /// <summary>
    /// Param Model
    /// </summary>
    public class ParamModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IList<string> Values { get; set; }

        /// <summary>
        /// Gets or sets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        public string[] Funcs { get; set; }
    }
}