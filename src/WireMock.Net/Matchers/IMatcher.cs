﻿namespace WireMock.Matchers
{
    /// <summary>
    /// IMatcher
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the match behaviour.
        /// </summary>
        MatchBehaviour MatchBehaviour { get; }

        /// <summary>
        /// Should this matcher throw an exception?
        /// </summary>
        bool ThrowException { get; }
    }
}