using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Azure.Sdk.Tools.LinkChecker.Functions
{
    public class PullRequestFunction
    {
        public PullRequestFunction(ILogger<PullRequestFunction> logger)
        {
            this.logger = logger;
        }

        private ILogger<PullRequestFunction> logger;

        [FunctionName("PullRequestFunction")]
        public async Task Run([EventHubTrigger("pull-request", Connection = "LinkCheckerEventHubConnectionString")] EventData @event)
        {
            logger.LogInformation("Tick!");
        }
    }
}
