using RestEase;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
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
        Task<StatusModel> PutSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Update the settings
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Post("__admin/settings")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PostSettingsAsync([Body] SettingsModel settings);

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
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PostMappingAsync([Body] MappingModel mapping);

        /// <summary>
        /// Delete all mappings.
        /// </summary>
        [Delete("__admin/mappings")]
        Task<StatusModel> DeleteMappingsAsync();

        /// <summary>
        /// Delete (reset) all mappings.
        /// </summary>
        [Post("__admin/mappings/reset")]
        Task<StatusModel> ResetMappingsAsync();

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
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PutMappingAsync([Path] Guid guid, [Body] MappingModel mapping);

        /// <summary>
        /// Delete a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        [Delete("__admin/mappings/{guid}")]
        Task<StatusModel> DeleteMappingAsync([Path] Guid guid);

        /// <summary>
        /// Save the mappings
        /// </summary>
        [Post("__admin/mappings/save")]
        Task<StatusModel> SaveMappingAsync();

        /// <summary>
        /// Get the requests.
        /// </summary>
        /// <returns>LogRequestModels</returns>
        [Get("__admin/requests")]
        Task<IList<LogEntryModel>> GetRequestsAsync();

        /// <summary>
        /// Delete all requests.
        /// </summary>
        [Delete("__admin/requests")]
        Task<StatusModel> DeleteRequestsAsync();

        /// <summary>
        /// Delete (reset) all requests.
        /// </summary>
        [Post("__admin/requests/reset")]
        Task<StatusModel> ResetRequestsAsync();

        /// <summary>
        /// Get a request based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <returns>MappingModel</returns>
        [Get("__admin/requests/{guid}")]
        Task<LogEntryModel> GetRequestAsync([Path] Guid guid);

        /// <summary>
        /// Delete a request based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        [Delete("__admin/requests/{guid}")]
        Task<StatusModel> DeleteRequestAsync([Path] Guid guid);

        /// <summary>
        /// Find a request based on the criteria
        /// </summary>
        /// <param name="model">The RequestModel</param>
        [Post("__admin/requests/find")]
        [Header("Content-Type", "application/json")]
        Task<IList<LogEntryModel>> FindRequestsAsync([Body] RequestModel model);

        /// <summary>
        /// Get all scenarios
        /// </summary>
        [Get("__admin/scenarios")]
        Task<IList<ScenarioState>> GetScenariosAsync();

        /// <summary>
        /// Delete (reset) all scenarios
        /// </summary>
        [Delete("__admin/scenarios")]
        Task<StatusModel> DeleteScenariosAsync();

        /// <summary>
        /// Delete (reset) all scenarios
        /// </summary>
        [Post("__admin/scenarios")]
        Task<StatusModel> ResetScenariosAsync();
    }
}