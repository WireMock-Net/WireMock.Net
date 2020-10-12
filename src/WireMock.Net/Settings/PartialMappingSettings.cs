using JetBrains.Annotations;

namespace WireMock.Settings
{
    /// <summary>
    /// PartialMappingSettings
    /// </summary>
    /// <seealso cref="IPartialMappingSettings" />
    public class PartialMappingSettings : IPartialMappingSettings
    {
        /// <inheritdoc cref="IPartialMappingSettings.EnforceHttpMethod"/>
        [PublicAPI]
        public bool EnforceHttpMethod { get; set; }
    }
}