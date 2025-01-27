// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Greet;
using Grpc.Net.Client;
using WireMock.Constants;
using WireMock.Net.Testcontainers;
using Xunit;

namespace WireMock.Net.Tests.Testcontainers;

public partial class TestcontainersTests
{
    [Fact]
    public async Task WireMockContainer_Build_TestGrpc_ProtoDefinitionFromJson_UsingGrpcGeneratedClient()
    {
        var wireMockContainer = await Given_WireMockContainerIsStartedForHttpAndGrpc();

        await Given_ProtoBufMappingIsAddedViaAdminInterfaceAsync(wireMockContainer);

        var adminClient = wireMockContainer.CreateWireMockAdminClient();

        var mappingModels = await adminClient.GetMappingsAsync();
        mappingModels.Should().NotBeNull().And.HaveCount(1);

        try
        {
            var x = await When_GrpcClient_Calls_SayHelloAsync(wireMockContainer);
        }
        catch (Exception e)
        {
            var logs = await wireMockContainer.GetLogsAsync(DateTime.MinValue);

            int tttt = 9;
            throw;
        }

        var reply = await When_GrpcClient_Calls_SayHelloAsync(wireMockContainer);

        Then_ReplyMessage_Should_BeCorrect(reply);

        await wireMockContainer.StopAsync();
    }

    private static async Task<WireMockContainer> Given_WireMockContainerIsStartedForHttpAndGrpc()
    {
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .AddUrl("grpc://*:9090")
            .Build();

        await wireMockContainer.StartAsync();

        return wireMockContainer;
    }

    private static async Task Given_ProtoBufMappingIsAddedViaAdminInterfaceAsync(WireMockContainer wireMockContainer)
    {
        var mappingsJson = ReadMappingFile("protobuf-mapping-1.json");

        using var httpClient = wireMockContainer.CreateClient();

        var result = await httpClient.PostAsync("/__admin/mappings", new StringContent(mappingsJson, Encoding.UTF8, WireMockConstants.ContentTypeJson));
        result.EnsureSuccessStatusCode();
    }

    private static async Task<HelloReply> When_GrpcClient_Calls_SayHelloAsync(WireMockContainer wireMockContainer)
    {
        var address = wireMockContainer.GetPublicUrls()[9090];
        var channel = GrpcChannel.ForAddress(address);

        var client = new Greeter.GreeterClient(channel);

        return await client.SayHelloAsync(new HelloRequest { Name = "stef" });
    }

    private static void Then_ReplyMessage_Should_BeCorrect(HelloReply reply)
    {
        reply.Message.Should().Be("hello stef POST");
    }

    private static string ReadMappingFile(string filename)
    {
        return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "__admin", "mappings", filename));
    }
}
#endif