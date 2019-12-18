using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SearchFunction
{
    public static class RefreshIndex
    {
        [FunctionName("refresh-search-index")]
        public static void Run([TimerTrigger("0 0 12 ? * FRI *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Load at: {DateTime.Now}");
        }
    }
}