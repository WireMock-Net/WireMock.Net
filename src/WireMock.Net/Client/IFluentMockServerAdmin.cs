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
        /// Update the settings.
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Put("__admin/settings")]
        [Header("Content-Type", "application/json")]
        Task<string> PutSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Update the settings
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Post("__admin/settings")]
        [Header("Content-Type", "application/json")]
        Task<string> PostSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Get the mappings.
        /// </summary>
        /// <returns>MappingModels</returns>
        [Get("__admin/mappings")]
        Task<IList<MappingModel>> GetMappingsAsync();

        /// <summary>
        /// Add a new mapping.
        /// </summary>
        /// <param name="mapping">MappingModel</param>
        [Post("__admin/mappings")]
        Task<string> PostMappingAsync([Body] MappingModel mapping);

        /// <summary>
        /// Delete all mappings.
        /// </summary>
        [Delete("__admin/mappings")]
        Task<string> DeleteMappingsAsync();

        /// <summary>
        /// Delete (reset) all mappings.
        /// </summary>
        [Post("__admin/mappings/reset")]
        Task<string> ResetMappingsAsync();

        /// <summary>
        /// Get a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <returns>MappingModel</returns>
        [Get("__admin/mappings/{guid}")]
        Task<MappingModel> GetMappingAsync([Path] Guid guid);

        /// <summary>
        /// Update a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <param name="mapping">MappingModel</param>
        [Put("__admin/mappings/{guid}")]
        Task<string> PutMappingAsync([Path] Guid guid, [Body] MappingModel mapping);

        /// <summary>
        /// Delete a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        [Delete("__admin/mappings/{guid}")]
        Task<string> DeleteMappingAsync([Path] Guid guid);

        /// <summary>
        /// Save the mappings
        /// </summary>
        [Post("__admin/mappings/save")]
        Task<string> SaveMappingAsync();
    }
}