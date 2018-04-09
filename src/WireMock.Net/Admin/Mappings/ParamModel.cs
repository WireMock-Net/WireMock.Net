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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public IList<string> Values { get; set; }

        ///// <summary>
        ///// Gets or sets the functions.
        ///// </summary>
        //public string[] Funcs { get; set; }
    }
}