// Copyright © WireMock.Net
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
using WireMock.Net.Testcontainers.Utils;
using Xunit;

namespace WireMock.Net.Tests.Facts;

public sealed class RunOnDockerPlatformFact : FactAttribute
{
    public RunOnDockerPlatformFact(string platform)
    {
        if (TestcontainersUtils.GetImageOSAsync.Value.Result != OSPlatform.Create(platform))
        {
            Skip = $"Only run test when Docker OS Platform {platform} is used.";
        }
    }
}
#endif