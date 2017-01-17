using System.Diagnostics.CodeAnalysis;

[module:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
        "SA1633:FileMustHaveHeader",
        Justification = "Reviewed. Suppression is OK here, as unknown copyright and company.")]

namespace WireMock
{
    /// <summary>
    /// The registration callback.
    /// </summary>
    /// <param name="route">
    /// The route.
    /// </param>
    public delegate void RegistrationCallback(Route route);
}
