// Copyright © WireMock.Net

using System.Threading.Tasks;
using AnyOfTypes;
using RestEase;
using WireMock.Org.Abstractions;

namespace WireMock.Org.RestClient;

/// <summary>
/// Summary: WireMockOrg
///
/// Title  : WireMock
/// Version: 2.3x
/// </summary>
public interface IWireMockOrgApi
{
    /// <summary>
    /// Get all stub mappings
    ///
    /// GetAdminMappings (/__admin/mappings)
    /// </summary>
    /// <param name="limit">The maximum number of results to return</param>
    /// <param name="offset">The start index of the results to return</param>
    [Get("/__admin/mappings")]
    Task<GetAdminMappingsResult> GetAdminMappingsAsync([Query] int? limit, [Query] int? offset);

    /// <summary>
    /// Create a new stub mapping
    ///
    /// PostAdminMappings (/__admin/mappings)
    /// </summary>
    [Post("/__admin/mappings")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminMappingsResult> PostAdminMappingsAsync();

    /// <summary>
    /// Delete all stub mappings
    ///
    /// DeleteAdminMappings (/__admin/mappings)
    /// </summary>
    [Delete("/__admin/mappings")]
    Task<object> DeleteAdminMappingsAsync();

    /// <summary>
    /// Reset stub mappings
    ///
    /// PostAdminMappingsReset (/__admin/mappings/reset)
    /// </summary>
    [Post("/__admin/mappings/reset")]
    Task<object> PostAdminMappingsResetAsync();

    /// <summary>
    /// Persist stub mappings
    ///
    /// PostAdminMappingsSave (/__admin/mappings/save)
    /// </summary>
    [Post("/__admin/mappings/save")]
    Task<object> PostAdminMappingsSaveAsync();

    /// <summary>
    /// Get stub mapping by ID
    ///
    /// GetAdminMappingsByStubMappingId (/__admin/mappings/{stubMappingId})
    /// </summary>
    /// <param name="stubMappingId">The UUID of stub mapping</param>
    [Get("/__admin/mappings/{stubMappingId}")]
    Task<Response<AnyOf<GetAdminMappingsByStubMappingIdResult, object>>> GetAdminMappingsByStubMappingIdAsync([Path] string stubMappingId);

    /// <summary>
    /// Update a stub mapping
    ///
    /// PutAdminMappingsByStubMappingId (/__admin/mappings/{stubMappingId})
    /// </summary>
    /// <param name="stubMappingId">The UUID of stub mapping</param>
    [Put("/__admin/mappings/{stubMappingId}")]
    [Header("Content-Type", "application/json")]
    Task<Response<AnyOf<PutAdminMappingsByStubMappingIdResult, object>>> PutAdminMappingsByStubMappingIdAsync([Path] string stubMappingId);

    /// <summary>
    /// Delete a stub mapping
    ///
    /// DeleteAdminMappingsByStubMappingId (/__admin/mappings/{stubMappingId})
    /// </summary>
    /// <param name="stubMappingId">The UUID of stub mapping</param>
    [Delete("/__admin/mappings/{stubMappingId}")]
    Task<object> DeleteAdminMappingsByStubMappingIdAsync([Path] string stubMappingId);

    /// <summary>
    /// Find stubs by matching on their metadata
    ///
    /// PostAdminMappingsFindByMetadata (/__admin/mappings/find-by-metadata)
    /// </summary>
    [Post("/__admin/mappings/find-by-metadata")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminMappingsFindByMetadataResult> PostAdminMappingsFindByMetadataAsync();

    /// <summary>
    /// Delete stub mappings matching metadata
    ///
    /// PostAdminMappingsRemoveByMetadata (/__admin/mappings/remove-by-metadata)
    /// </summary>
    [Post("/__admin/mappings/remove-by-metadata")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminMappingsRemoveByMetadataAsync();

    /// <summary>
    /// Get all requests in journal
    ///
    /// GetAdminRequests (/__admin/requests)
    /// </summary>
    /// <param name="limit">The maximum number of results to return</param>
    /// <param name="since">Only return logged requests after this date</param>
    [Get("/__admin/requests")]
    Task<object> GetAdminRequestsAsync([Query] string limit, [Query] string since);

    /// <summary>
    /// Delete all requests in journal
    ///
    /// DeleteAdminRequests (/__admin/requests)
    /// </summary>
    [Delete("/__admin/requests")]
    Task<object> DeleteAdminRequestsAsync();

    /// <summary>
    /// Get request by ID
    ///
    /// GetAdminRequestsByRequestId (/__admin/requests/{requestId})
    /// </summary>
    /// <param name="requestId">The UUID of the logged request</param>
    [Get("/__admin/requests/{requestId}")]
    Task<object> GetAdminRequestsByRequestIdAsync([Path] string requestId);

    /// <summary>
    /// Delete request by ID
    ///
    /// DeleteAdminRequestsByRequestId (/__admin/requests/{requestId})
    /// </summary>
    /// <param name="requestId">The UUID of the logged request</param>
    [Delete("/__admin/requests/{requestId}")]
    Task<object> DeleteAdminRequestsByRequestIdAsync([Path] string requestId);

    /// <summary>
    /// Empty the request journal
    ///
    /// PostAdminRequestsReset (/__admin/requests/reset)
    /// </summary>
    [Post("/__admin/requests/reset")]
    Task<object> PostAdminRequestsResetAsync();

    /// <summary>
    /// Count requests by criteria
    ///
    /// PostAdminRequestsCount (/__admin/requests/count)
    /// </summary>
    [Post("/__admin/requests/count")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminRequestsCountResult> PostAdminRequestsCountAsync();

    /// <summary>
    /// Remove requests by criteria
    ///
    /// PostAdminRequestsRemove (/__admin/requests/remove)
    /// </summary>
    [Post("/__admin/requests/remove")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminRequestsRemoveAsync();

    /// <summary>
    /// Delete requests mappings matching metadata
    ///
    /// PostAdminRequestsRemoveByMetadata (/__admin/requests/remove-by-metadata)
    /// </summary>
    [Post("/__admin/requests/remove-by-metadata")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminRequestsRemoveByMetadataAsync();

    /// <summary>
    /// Find requests by criteria
    ///
    /// PostAdminRequestsFind (/__admin/requests/find)
    /// </summary>
    [Post("/__admin/requests/find")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminRequestsFindAsync();

    /// <summary>
    /// Find unmatched requests
    ///
    /// GetAdminRequestsUnmatched (/__admin/requests/unmatched)
    /// </summary>
    [Get("/__admin/requests/unmatched")]
    Task<object> GetAdminRequestsUnmatchedAsync();

    /// <summary>
    /// Retrieve near-misses for all unmatched requests
    ///
    /// GetAdminRequestsUnmatchedNearMisses (/__admin/requests/unmatched/near-misses)
    /// </summary>
    [Get("/__admin/requests/unmatched/near-misses")]
    Task<GetAdminRequestsUnmatchedNearMissesResult> GetAdminRequestsUnmatchedNearMissesAsync();

    /// <summary>
    /// Find near misses matching specific request
    ///
    /// PostAdminNearMissesRequest (/__admin/near-misses/request)
    /// </summary>
    [Post("/__admin/near-misses/request")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminNearMissesRequestResult> PostAdminNearMissesRequestAsync();

    /// <summary>
    /// Find near misses matching request pattern
    ///
    /// PostAdminNearMissesRequestPattern (/__admin/near-misses/request-pattern)
    /// </summary>
    [Post("/__admin/near-misses/request-pattern")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminNearMissesRequestPatternResult> PostAdminNearMissesRequestPatternAsync();

    /// <summary>
    /// Start recording
    ///
    /// PostAdminRecordingsStart (/__admin/recordings/start)
    /// </summary>
    [Post("/__admin/recordings/start")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminRecordingsStartAsync();

    /// <summary>
    /// Stop recording
    ///
    /// PostAdminRecordingsStop (/__admin/recordings/stop)
    /// </summary>
    [Post("/__admin/recordings/stop")]
    Task<PostAdminRecordingsStopResult> PostAdminRecordingsStopAsync();

    /// <summary>
    /// Get recording status
    ///
    /// GetAdminRecordingsStatus (/__admin/recordings/status)
    /// </summary>
    [Get("/__admin/recordings/status")]
    Task<GetAdminRecordingsStatusResult> GetAdminRecordingsStatusAsync();

    /// <summary>
    /// Take a snapshot recording
    ///
    /// PostAdminRecordingsSnapshot (/__admin/recordings/snapshot)
    /// </summary>
    [Post("/__admin/recordings/snapshot")]
    [Header("Content-Type", "application/json")]
    Task<PostAdminRecordingsSnapshotResult> PostAdminRecordingsSnapshotAsync();

    /// <summary>
    /// Get all scenarios
    ///
    /// GetAdminScenarios (/__admin/scenarios)
    /// </summary>
    [Get("/__admin/scenarios")]
    Task<GetAdminScenariosResult> GetAdminScenariosAsync();

    /// <summary>
    /// Reset the state of all scenarios
    ///
    /// PostAdminScenariosReset (/__admin/scenarios/reset)
    /// </summary>
    [Post("/__admin/scenarios/reset")]
    Task<object> PostAdminScenariosResetAsync();

    /// <summary>
    /// Update global settings
    ///
    /// PostAdminSettings (/__admin/settings)
    /// </summary>
    [Post("/__admin/settings")]
    [Header("Content-Type", "application/json")]
    Task<object> PostAdminSettingsAsync();

    /// <summary>
    /// Reset mappings and request journal
    ///
    /// PostAdminReset (/__admin/reset)
    /// </summary>
    [Post("/__admin/reset")]
    Task<object> PostAdminResetAsync();

    /// <summary>
    /// Shutdown the WireMock server
    ///
    /// PostAdminShutdown (/__admin/shutdown)
    /// </summary>
    [Post("/__admin/shutdown")]
    Task<object> PostAdminShutdownAsync();
}