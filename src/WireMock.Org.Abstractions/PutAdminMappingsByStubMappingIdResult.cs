// Copyright © WireMock.Net

namespace WireMock.Org.Abstractions
{
    public class PutAdminMappingsByStubMappingIdResult
    {
        /// <summary>
        /// This stub mapping's unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Alias for the id
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// The stub mapping's name
        /// </summary>
        public string Name { get; set; }

        public WireMockOrgRequest Request { get; set; }

        public WireMockOrgResponse Response { get; set; }

        /// <summary>
        /// Indicates that the stub mapping should be persisted immediately on create/update/delete and survive resets to default.
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// This stub mapping's priority relative to others. 1 is highest.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// The name of the scenario that this stub mapping is part of
        /// </summary>
        public string ScenarioName { get; set; }

        /// <summary>
        /// The required state of the scenario in order for this stub to be matched.
        /// </summary>
        public string RequiredScenarioState { get; set; }

        /// <summary>
        /// The new state for the scenario to be updated to after this stub is served.
        /// </summary>
        public string NewScenarioState { get; set; }

        /// <summary>
        /// A map of the names of post serve action extensions to trigger and their parameters.
        /// </summary>
        public object PostServeActions { get; set; }

        /// <summary>
        /// Arbitrary metadata to be used for e.g. tagging, documentation. Can also be used to find and remove stubs.
        /// </summary>
        public object Metadata { get; set; }
    }
}