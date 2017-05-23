using RestEase;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WireMock.Admin.Mappings;
using WireMock.Admin.Settings;

namespace WireMock.Client
{
    /// <summary>
    /// The RestEase interface which defines all admin commands.
    /// </summary>
    public interface IFluentMockServerAdmin
    {
        /// <summary>
        /// Authentication header
        /// </summary>
        [Header("Authorization")]
        AuthenticationHeaderValue Authorization { get; set; }

        /// <summary>
        /// Get the settings.
        /// </summary>
        /// <returns>SettingsModel</returns>
        [Get("__admin/settings")]
        Task<SettingsModel> GetSettingsAsync();

        /// <summary>
        /// Post the settings
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Post("__admin/settings")]
        Task PostSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Get the mappings.
        /// </summary>
        /// <returns>MappingModels</returns>
        [Get("__admin/mappings")]
        Task<IList<MappingModel>> GetMappingsAsync();

        /// <summary>
        /// Get a mappings based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <returns>MappingModel</returns>
        [Get("__admin/mappings/{guid}")]
        Task<MappingModel> GetMappingAsync([Path] Guid guid);
    }
}