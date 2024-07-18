// Copyright © WireMock.Net

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WireMockAzureQueueExample;

public class Function1
{
    [FunctionName("Function1")]
    public void Run([QueueTrigger("myqueue-items", Connection = "ConnectionStringToWireMock")]string myQueueItem, ILogger log)
    {
        log.LogWarning($"C# Queue trigger function processed: {myQueueItem}");
    }
}