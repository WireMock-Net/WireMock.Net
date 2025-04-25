// Copyright © WireMock.Net

namespace WireMock.Net.TestWebApplication;

public class TestBackgroundService(HttpClient client, TaskQueue taskQueue, ILogger<TestBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in taskQueue.ReadTasks(stoppingToken))
        {
            try
            {
                var result = await client.GetStringAsync(item, stoppingToken);
                await taskQueue.WriteResponse(result, stoppingToken);
            }
            catch (ArgumentNullException argNullEx)
            {
                logger.LogError(argNullEx, "Null exception");
                await taskQueue.WriteErrorResponse(argNullEx.Message, stoppingToken);
            }
        }
    }
}