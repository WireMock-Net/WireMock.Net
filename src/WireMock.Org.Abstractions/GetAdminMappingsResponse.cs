using System;
using System.Collections.Generic;

namespace WireMockOrg.Models
{
    public class GetAdminMappingsResponse
    {
        public MappingsModel[] Mappings { get; set; }

        public object Meta { get; set; }
    }
}
