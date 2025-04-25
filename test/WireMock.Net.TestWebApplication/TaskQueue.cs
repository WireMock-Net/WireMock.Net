// Copyright © WireMock.Net

using System.Threading.Channels;

namespace WireMock.Net.TestWebApplication;

public class TaskQueue
{
    private enum Status
    {
        Success,
        Error
    }

    private readonly Channel<string> _taskChannel = Channel.CreateUnbounded<string>();
    private readonly Channel<(Status, string)> _responseChannel = Channel.CreateUnbounded<(Status, string)>();

    public async Task<string> Enqueue(string taskId, CancellationToken cancellationToken)
    {
        await _taskChannel.Writer.WriteAsync(taskId, cancellationToken);
        var (status, result) = await _responseChannel.Reader.ReadAsync(cancellationToken);
        if (status == Status.Error)
        {
            throw new InvalidOperationException($"Received an error response from the task processor: ${result}");
        }

        return result;
    }

    public IAsyncEnumerable<string> ReadTasks(CancellationToken stoppingToken) =>
        _taskChannel.Reader.ReadAllAsync(stoppingToken);

    public async Task WriteResponse(string result, CancellationToken stoppingToken) =>
        await _responseChannel.Writer.WriteAsync((Status.Success, result), stoppingToken);

    public async Task WriteErrorResponse(string result, CancellationToken stoppingToken) =>
        await _responseChannel.Writer.WriteAsync((Status.Error, result), stoppingToken);
}