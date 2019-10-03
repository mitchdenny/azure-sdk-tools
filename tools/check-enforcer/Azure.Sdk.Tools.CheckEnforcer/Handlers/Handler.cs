using Azure.Sdk.Tools.CheckEnforcer.Configuration;
using Azure.Sdk.Tools.CheckEnforcer.Integrations.GitHub;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Sdk.Tools.CheckEnforcer.Handlers
{
    public abstract class Handler<T> where T: ActivityPayload
    {
        public Handler(IGlobalConfigurationProvider globalConfigurationProvider, IGitHubClientProvider gitHubClientProvider, IRepositoryConfigurationProvider repositoryConfigurationProvider, IDurableEntityClient entityClient, ILogger logger)
        {
            this.GlobalConfigurationProvider = globalConfigurationProvider;
            this.GitHubClientProvider = gitHubClientProvider;
            this.RepositoryConfigurationProvider = repositoryConfigurationProvider;
            this.EntityClient = entityClient;
            this.Logger = logger;
        }

        protected IDurableEntityClient EntityClient { get; private set; }
        protected IGlobalConfigurationProvider GlobalConfigurationProvider { get; private set; }
        protected IGitHubClientProvider GitHubClientProvider { get; private set; }
        protected IRepositoryConfigurationProvider RepositoryConfigurationProvider { get; private set; }


        protected ILogger Logger { get; private set; }

        protected async Task SetSuccessAsync(GitHubClient client, long repositoryId, string sha, CancellationToken cancellationToken)
        {
            var response = await client.Check.Run.GetAllForReference(repositoryId, sha);
            var runs = response.CheckRuns;
            var run = runs.Single(r => r.Name == this.GlobalConfigurationProvider.GetApplicationName());

            await client.Check.Run.Update(repositoryId, run.Id, new CheckRunUpdate()
            {
                Conclusion = new StringEnum<CheckConclusion>(CheckConclusion.Success),
                CompletedAt = DateTimeOffset.UtcNow
            });
        }

        protected async Task SetInProgressAsync(GitHubClient client, long repositoryId, string sha, CancellationToken cancellationToken)
        {
            var response = await client.Check.Run.GetAllForReference(repositoryId, sha);
            var runs = response.CheckRuns;
            var run = runs.Single(r => r.Name == this.GlobalConfigurationProvider.GetApplicationName());

            await client.Check.Run.Update(repositoryId, run.Id, new CheckRunUpdate()
            {
                Status = new StringEnum<CheckStatus>(CheckStatus.InProgress)
            });
        }

        protected async Task SetQueuedAsync(GitHubClient client, long repositoryId, string sha, CancellationToken cancellationToken)
        {
            var response = await client.Check.Run.GetAllForReference(repositoryId, sha);
            var runs = response.CheckRuns;
            var run = runs.Single(r => r.Name == this.GlobalConfigurationProvider.GetApplicationName());

            await client.Check.Run.Update(repositoryId, run.Id, new CheckRunUpdate()
            {
                Status = new StringEnum<CheckStatus>(CheckStatus.Queued)
            });
        }


        private T DeserializePayload(string json)
        {
            Logger.LogTrace("Payload: {json}", json);

            SimpleJsonSerializer serializer = new SimpleJsonSerializer();
            var payload = serializer.Deserialize<T>(json);
            return payload;
        }

        public async Task HandleAsync(string json, CancellationToken cancellationToken)
        {
            var deserializedPayload = DeserializePayload(json);
            var installationId = deserializedPayload.Installation.Id;

            var client = await this.GitHubClientProvider.GetInstallationClientAsync(installationId, cancellationToken);
            var context = new HandlerContext<T>(deserializedPayload, client);

            await HandleCoreAsync(context, cancellationToken);
        }

        protected abstract Task HandleCoreAsync(HandlerContext<T> context, CancellationToken cancellationToken);
    }
}
