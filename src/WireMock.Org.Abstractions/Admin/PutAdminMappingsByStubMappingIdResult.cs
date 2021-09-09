using System;
using System.Collections.Generic;

namespace WireMockOrg.Models
{
    public class PutAdminMappingsByStubMappingIdResult
    {
        public string Id { get; set; }

        public string Uuid { get; set; }

        public string Name { get; set; }

        public object Request { get; set; }

        public bool Persistent { get; set; }

        public int Priority { get; set; }

        public string ScenarioName { get; set; }

        public string RequiredScenarioState { get; set; }

        public string NewScenarioState { get; set; }

        public object PostServeActions { get; set; }

        public object Metadata { get; set; }
    }
}
