// Copyright © WireMock.Net

namespace WireMock.Net.Testcontainers.Models;

internal record ContainerInfo
(
    string Image,
    string MappingsPath
);