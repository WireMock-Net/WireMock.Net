using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class PostAdminRecordingsSnapshotResponse
    {
        public MappingsModel[] Mappings { get; set; }

        public object Meta { get; set; }
    }
}
