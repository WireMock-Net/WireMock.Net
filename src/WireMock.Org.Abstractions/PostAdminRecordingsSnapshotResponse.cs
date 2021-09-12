using System;
using System.Collections.Generic;

namespace WireMock.Org.Abstractions
{
    public class PostAdminRecordingsSnapshotResponse
    {
        public Mapping[] Mappings { get; set; }

        public Meta Meta { get; set; }
    }
}
