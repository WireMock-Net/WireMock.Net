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
        /// Gets or sets the matchers.
        /// </summary>
        public MatcherModel[] Matchers { get; set; }
    }
}