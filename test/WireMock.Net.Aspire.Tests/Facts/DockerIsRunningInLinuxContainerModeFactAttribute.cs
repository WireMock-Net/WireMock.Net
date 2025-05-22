// Copyright © WireMock.Net

namespace WireMock.Net.Aspire.Tests.Facts;

public sealed class DockerIsRunningInLinuxContainerModeFactAttribute : FactAttribute
{
    private const string SkipReason = "Docker is not running in Linux container mode. Skipping test.";

    public DockerIsRunningInLinuxContainerModeFactAttribute()
    {
        if (!DockerUtils.IsDockerRunningLinuxContainerMode.Value)
        {
            Skip = SkipReason;
        }
    }
}