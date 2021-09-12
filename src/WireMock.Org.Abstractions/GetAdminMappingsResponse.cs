using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class GetAdminMappingsResponse
    {
        public Mappings[] Mappings { get; set; }

        public Meta Meta { get; set; }
    }
}
