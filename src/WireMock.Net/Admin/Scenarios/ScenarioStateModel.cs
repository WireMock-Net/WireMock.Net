﻿namespace WireMock.Admin.Scenarios
{
    /// <summary>
    /// ScenarioStateModel
    /// </summary>
    public class ScenarioStateModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the NextState.
        /// </summary>
        public string NextState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScenarioStateModel"/> is started.
        /// </summary>
        public bool Started { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScenarioStateModel"/> is finished.
        /// </summary>
        public bool Finished { get; set; }
    }
}