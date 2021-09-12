using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class PostAdminMappingsFindByMetadataResponse
    {
        public Mappings[] Mappings { get; set; }

        public Meta Meta { get; set; }
    }
}
