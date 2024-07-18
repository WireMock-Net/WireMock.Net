// Copyright © WireMock.Net

using Greet;
using Grpc.Net.Client;

namespace WireMock.Net.Console.GrpcClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        var channel = GrpcChannel.ForAddress("http://localhost:9093/grpc3", new GrpcChannelOptions
        {
            Credentials = Grpc.Core.ChannelCredentials.Insecure
        });

        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest { Name = "stef" });

        System.Console.WriteLine("Greeting: " + reply.Message);
    }
}