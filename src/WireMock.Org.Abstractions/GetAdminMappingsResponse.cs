using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class GetAdminMappingsResponse
    {
        public Mapping[] Mappings { get; set; }

        public Meta Meta { get; set; }
    }
}
