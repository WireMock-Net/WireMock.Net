// Copyright © WireMock.Net

namespace WireMock
{
    /// <summary>
    /// The registration callback.
    /// </summary>
    /// <param name="mapping">The mapping.</param>
    /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
    public delegate void RegistrationCallback(IMapping mapping, bool saveToFile = false);
}