using WireMock.Client.Builders;

namespace WireMock.Client.Extensions;

/// <summary>
/// Some extensions for <see cref="IWireMockAdminApi"/>.
/// </summary>
public static class WireMockAdminApiExtensions
{
    /// <summary>
    /// Get a new <see cref="AdminApiMappingBuilder"/> for the <see cref="IWireMockAdminApi"/>.
    /// </summary>
    /// <param name="api">See <see cref="IWireMockAdminApi"/>.</param>
    /// <returns></returns>
    public static AdminApiMappingBuilder GetMappingBuilder(this IWireMockAdminApi api)
    {
        return new AdminApiMappingBuilder(api);
    }
}