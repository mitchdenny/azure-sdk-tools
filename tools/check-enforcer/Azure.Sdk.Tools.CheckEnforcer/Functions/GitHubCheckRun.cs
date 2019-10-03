using Azure.Sdk.Tools.CheckEnforcer.Configuration;
using Azure.Sdk.Tools.CheckEnforcer.Integrations.GitHub;
using DurableTask.Core;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Octokit;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Sdk.Tools.CheckEnforcer.Functions
{
    public static class GitHubCheckRun
    {
        //private IGlobalConfigurationProvider globalConfigurationProvider;
        //private IGitHubClientProvider gitHubClientProvider;
        //private IRepositoryConfigurationProvider repositoryConfigurationProvider;

        //public GitHubCheckRun(IGlobalConfigurationProvider globalConfigurationProvider, IGitHubClientProvider gitHubClientProvider, IRepositoryConfigurationProvider repositoryConfigurationProvider)
        //{
        //    this.globalConfigurationProvider = globalConfigurationProvider;
        //    this.gitHubClientProvider = gitHubClientProvider;
        //    this.repositoryConfigurationProvider = repositoryConfigurationProvider;
        //}

        //protected async Task EvaluatePullRequestAsync(GitHubClient client, long installationId, long repositoryId, string sha, CancellationToken cancellationToken)
        //{
        //    var configuration = await repositoryConfigurationProvider.GetRepositoryConfigurationAsync(installationId, repositoryId, sha, cancellationToken);

        //    if (configuration.IsEnabled)
        //    {
        //        var runsResponse = await client.Check.Run.GetAllForReference(repositoryId, sha);
        //        var runs = runsResponse.CheckRuns;

        //        // NOTE: If this blows up it means that we didn't receive the check_suite request.
        //        var checkEnforcerRun = await CreateCheckAsync(client, repositoryId, sha, false, cancellationToken);

        //        var otherRuns = from run in runs
        //                        where run.Name != globalConfigurationProvider.GetApplicationName()
        //                        select run;

        //        var totalOtherRuns = otherRuns.Count();

        //        var outstandingOtherRuns = from run in otherRuns
        //                                   where run.Conclusion != new StringEnum<CheckConclusion>(CheckConclusion.Success)
        //                                   select run;

        //        var totalOutstandingOtherRuns = outstandingOtherRuns.Count();

        //        if (totalOtherRuns >= configuration.MinimumCheckRuns && totalOutstandingOtherRuns == 0 && checkEnforcerRun.Conclusion != new StringEnum<CheckConclusion>(CheckConclusion.Success))
        //        {
        //            await client.Check.Run.Update(repositoryId, checkEnforcerRun.Id, new CheckRunUpdate()
        //            {
        //                Conclusion = new StringEnum<CheckConclusion>(CheckConclusion.Success),
        //                Status = new StringEnum<CheckStatus>(CheckStatus.Completed),
        //                CompletedAt = DateTimeOffset.UtcNow
        //            });
        //        }
        //        else if (checkEnforcerRun.Conclusion == new StringEnum<CheckConclusion>(CheckConclusion.Success))
        //        {
        //            // NOTE: We do this when we need to go back from a conclusion of success to a status of in-progress.
        //            await CreateCheckAsync(client, repositoryId, sha, true, cancellationToken);
        //        }
        //    }
        //}
        //private async Task<CheckRun> CreateCheckAsync(GitHubClient client, long repositoryId, string headSha, bool recreate, CancellationToken cancellationToken)
        //{
        //    var response = await client.Check.Run.GetAllForReference(repositoryId, headSha);
        //    var runs = response.CheckRuns;
        //    var checkEnforcerRuns = runs.Where(r => r.Name == globalConfigurationProvider.GetApplicationName());

        //    if (checkEnforcerRuns.Count() > 1)
        //    {
        //        var firstRun = checkEnforcerRuns.First();
        //        SimpleJsonSerializer serializer = new SimpleJsonSerializer();
        //        var serializedFirstRun = serializer.Serialize(firstRun);
        //    }

        //    var checkRun = checkEnforcerRuns.SingleOrDefault();

        //    if (checkRun == null || recreate)
        //    {
        //        checkRun = await client.Check.Run.Create(
        //            repositoryId,
        //            new NewCheckRun(globalConfigurationProvider.GetApplicationName(), headSha)
        //            {
        //                Status = new StringEnum<CheckStatus>(CheckStatus.InProgress),
        //                StartedAt = DateTimeOffset.UtcNow
        //            }
        //        );
        //    }

        //    return checkRun;
        //}

        [FunctionName("GHPR")]
        public static void Run([EntityTrigger] IDurableEntityContext ctx)
        {
            var state = ctx.GetState<int>(() => 0);
            state++;
            ctx.SetState(state);
        }
    }
}
