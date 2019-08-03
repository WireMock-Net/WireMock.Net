using RestEase;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WireMock.Models.Mappings;
using WireMock.Models.Requests;
using WireMock.Models.Scenarios;
using WireMock.Models.Settings;

namespace WireMock.Client
{
    /// <summary>
    /// The RestEase interface which defines all admin commands.
    /// </summary>
    [BasePath("__admin")]
    public interface IWireMockAdminApi
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
        [Get("settings")]
        Task<SettingsModel> GetSettingsAsync();

        /// <summary>
        /// Update the settings.
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Put("settings")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PutSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Update the settings
        /// </summary>
        /// <param name="settings">SettingsModel</param>
        [Post("settings")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PostSettingsAsync([Body] SettingsModel settings);

        /// <summary>
        /// Get the mappings.
        /// </summary>
        /// <returns>MappingModels</returns>
        [Get("mappings")]
        Task<IList<MappingModel>> GetMappingsAsync();

        /// <summary>
        /// Add a new mapping.
        /// </summary>
        /// <param name="mapping">MappingModel</param>
        [Post("mappings")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PostMappingAsync([Body] MappingModel mapping);

        /// <summary>
        /// Add new mappings.
        /// </summary>
        /// <param name="mappings">MappingModels</param>
        [Post("mappings")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PostMappingsAsync([Body] IList<MappingModel> mappings);

        /// <summary>
        /// Delete all mappings.
        /// </summary>
        [Delete("mappings")]
        Task<StatusModel> DeleteMappingsAsync();

        /// <summary>
        /// Delete (reset) all mappings.
        /// </summary>
        [Post("mappings/reset")]
        Task<StatusModel> ResetMappingsAsync();

        /// <summary>
        /// Get a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <returns>MappingModel</returns>
        [Get("mappings/{guid}")]
        Task<MappingModel> GetMappingAsync([Path] Guid guid);

        /// <summary>
        /// Update a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <param name="mapping">MappingModel</param>
        [Put("mappings/{guid}")]
        [Header("Content-Type", "application/json")]
        Task<StatusModel> PutMappingAsync([Path] Guid guid, [Body] MappingModel mapping);

        /// <summary>
        /// Delete a mapping based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        [Delete("mappings/{guid}")]
        Task<StatusModel> DeleteMappingAsync([Path] Guid guid);

        /// <summary>
        /// Save the mappings
        /// </summary>
        [Post("mappings/save")]
        Task<StatusModel> SaveMappingAsync();

        /// <summary>
        /// Get the requests.
        /// </summary>
        /// <returns>LogRequestModels</returns>
        [Get("requests")]
        Task<IList<LogEntryModel>> GetRequestsAsync();

        /// <summary>
        /// Delete all requests.
        /// </summary>
        [Delete("requests")]
        Task<StatusModel> DeleteRequestsAsync();

        /// <summary>
        /// Delete (reset) all requests.
        /// </summary>
        [Post("requests/reset")]
        Task<StatusModel> ResetRequestsAsync();

        /// <summary>
        /// Get a request based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        /// <returns>MappingModel</returns>
        [Get("requests/{guid}")]
        Task<LogEntryModel> GetRequestAsync([Path] Guid guid);

        /// <summary>
        /// Delete a request based on the guid
        /// </summary>
        /// <param name="guid">The Guid</param>
        [Delete("requests/{guid}")]
        Task<StatusModel> DeleteRequestAsync([Path] Guid guid);

        /// <summary>
        /// Find a request based on the criteria
        /// </summary>
        /// <param name="model">The RequestModel</param>
        [Post("requests/find")]
        [Header("Content-Type", "application/json")]
        Task<IList<LogEntryModel>> FindRequestsAsync([Body] RequestModel model);

        /// <summary>
        /// Get all scenarios
        /// </summary>
        [Get("scenarios")]
        Task<IList<ScenarioStateModel>> GetScenariosAsync();

        /// <summary>
        /// Delete (reset) all scenarios
        /// </summary>
        [Delete("scenarios")]
        Task<StatusModel> DeleteScenariosAsync();

        /// <summary>
        /// Delete (reset) all scenarios
        /// </summary>
        [Post("scenarios")]
        Task<StatusModel> ResetScenariosAsync();

        /// <summary>
        /// Create a new File
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="body">The body</param>
        [Post("files/{filename}")]
        Task<StatusModel> PostFileAsync([Path] string filename, [Body] string body);

        /// <summary>
        /// Update an existing File
        /// </summary>
        /// <param name="filename">The filename</param>
        /// <param name="body">The body</param>
        [Put("files/{filename}")]
        Task<StatusModel> PutFileAsync([Path] string filename, [Body] string body);

        /// <summary>
        /// Get the content of an existing File
        /// </summary>
        /// <param name="filename">The filename</param>
        [Get("files/{filename}")]
        Task<string> GetFileAsync([Path] string filename);

        /// <summary>
        /// Delete an existing File
        /// </summary>
        /// <param name="filename">The filename</param>
        [Delete("files/{filename}")]
        Task<StatusModel> DeleteFileAsync([Path] string filename);

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="filename">The filename</param>
        [Head("files/{filename}")]
        Task FileExistsAsync([Path] string filename);
    }
}