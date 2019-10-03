using Azure.Sdk.Tools.CheckEnforcer.Configuration;
using Azure.Sdk.Tools.CheckEnforcer.Functions;
using Azure.Sdk.Tools.CheckEnforcer.Integrations.GitHub;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Sdk.Tools.CheckEnforcer.Handlers
{
    public class CheckRunHandler : Handler<CheckRunEventPayload>
    {
        public CheckRunHandler(
            IGlobalConfigurationProvider globalConfigurationProvider,
            IGitHubClientProvider gitHubClientProvider,
            IRepositoryConfigurationProvider repositoryConfigurationProvider,
            IDurableEntityClient entityClient, ILogger logger) : base(
                globalConfigurationProvider,
                gitHubClientProvider,
                repositoryConfigurationProvider,
                entityClient,
                logger)
        {
        }


        protected override async Task HandleCoreAsync(HandlerContext<CheckRunEventPayload> context, CancellationToken cancellationToken)
        {
            var payload = context.Payload;

            if (payload.CheckRun.Name != this.GlobalConfigurationProvider.GetApplicationName())
            {
                // Extract critical info for payload.
                var installationId = payload.Installation.Id;
                var repositoryId = payload.Repository.Id;
                var sha = payload.CheckRun.CheckSuite.HeadSha;

                var configuration = await this.RepositoryConfigurationProvider.GetRepositoryConfigurationAsync(
                    installationId,
                    repositoryId,
                    sha,
                    cancellationToken
                    );

                if (configuration.IsEnabled)
                {
                    var target = new GitHubCheckRunTarget()
                    {
                        Intent = GitHubCheckRunTargetIntent.Evaluate,
                        InstallationId = installationId,
                        RepositoryId = repositoryId,
                        Sha = sha
                    };

                    var entityId = new EntityId("GHPR", target.ToString());

                    await this.EntityClient.SignalEntityAsync(entityId, "Evaluate");
                }
            }
        }
    }
}
