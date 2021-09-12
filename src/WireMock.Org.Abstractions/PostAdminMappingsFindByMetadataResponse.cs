using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class PostAdminMappingsFindByMetadataResponse
    {
        public MappingsModel[] Mappings { get; set; }

        public object Meta { get; set; }
    }
}
