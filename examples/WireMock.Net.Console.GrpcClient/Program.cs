using Greet;
using Grpc.Net.Client;

namespace WireMock.Net.GrpcClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        // The port number must match the port of the gRPC server.
        //using var channel = GrpcChannel.ForAddress("http://localhost:9093");

        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var channel = GrpcChannel.ForAddress("http://localhost:9093", new GrpcChannelOptions
        {
            HttpHandler = handler,
            Credentials = Grpc.Core.ChannelCredentials.Insecure
        });

        var client = new Greeter.GreeterClient(channel);

        var reply = await client.SayHelloAsync(new HelloRequest { Name = "stef" });

        Console.WriteLine("Greeting: " + reply.Message);
    }
}