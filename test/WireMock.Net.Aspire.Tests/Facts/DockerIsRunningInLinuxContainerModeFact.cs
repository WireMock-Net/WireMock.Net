// Copyright © WireMock.Net

namespace WireMock.Net.Aspire.Tests.Facts;

public sealed class DockerIsRunningInLinuxContainerModeFact : FactAttribute
{
    private const string SkipReason = "Docker is not running in Linux container mode. Skipping test.";

    public DockerIsRunningInLinuxContainerModeFact()
    {
        if (!DockerUtils.IsDockerRunningLinuxContainerMode.Value)
        {
            Skip = SkipReason;
        }
    }
}