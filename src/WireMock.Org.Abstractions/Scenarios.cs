// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions
{
    public class Scenarios
    {
        /// <summary>
        /// The scenario ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The scenario name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All the states this scenario can be in
        /// </summary>
        public string[] PossibleStates { get; set; }

        /// <summary>
        /// The current state of this scenario
        /// </summary>
        public string State { get; set; }
    }
}