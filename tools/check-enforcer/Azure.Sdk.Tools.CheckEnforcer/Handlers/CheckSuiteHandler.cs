using Azure.Sdk.Tools.CheckEnforcer.Configuration;
using Azure.Sdk.Tools.CheckEnforcer.Functions;
using Azure.Sdk.Tools.CheckEnforcer.Integrations.GitHub;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Sdk.Tools.CheckEnforcer.Handlers
{
    public class CheckSuiteHandler : Handler<CheckSuiteEventPayload>
    {
        public CheckSuiteHandler(
            IGlobalConfigurationProvider globalConfigurationProvider,
            IGitHubClientProvider gitHubClientProvider,
            IRepositoryConfigurationProvider repositoryConfigurationProvider,
            IDurableEntityClient entityClient,
            ILogger logger) : base(
                globalConfigurationProvider,
                gitHubClientProvider,
                repositoryConfigurationProvider,
                entityClient,
                logger)
        {
        }

        protected override async Task HandleCoreAsync(HandlerContext<CheckSuiteEventPayload> context, CancellationToken cancellationToken)
        {
            var payload = context.Payload;

            if (payload.Action == "requested" || payload.Action == "rerequested")
            {
                var installationId = payload.Installation.Id;
                var repositoryId = payload.Repository.Id;
                var sha = payload.CheckSuite.HeadSha;

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
                        Intent = GitHubCheckRunTargetIntent.Create,
                        InstallationId = installationId,
                        RepositoryId = repositoryId,
                        Sha = sha
                    };

                    var entityId = new EntityId(nameof(GitHubCheckRun), target.ToString());

                    //await this.EntityClient.SignalEntityAsync<IGitHubCheckRun>(entityId, (ghcr) =>
                    //{
                    //    ghcr.Evaluate(target);
                    //});
                }
            }
        }
    }
}
