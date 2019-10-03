using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Sdk.Tools.CheckEnforcer
{
    public interface IGitHubWebhookProcessor
    {
        Task ProcessWebhookAsync(HttpRequest request, IDurableEntityClient entityClient, ILogger logger, CancellationToken cancellationToken);
    }
}
