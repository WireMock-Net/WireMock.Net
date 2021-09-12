using System;
using System.Collections.Generic;

namespace WireMockOrg.Models
{
    public class ScenariosModel
    {
        /// <summary>
        /// The scenario ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The scenario name
        /// </summary>
        public string Name { get; set; }

        public string PossibleStates { get; set; }

        /// <summary>
        /// The current state of this scenario
        /// </summary>
        public string State { get; set; }
    }
}
