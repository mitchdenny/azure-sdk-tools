using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Sdk.Tools.CheckEnforcer.Functions
{
    public class GitHubCheckRunTarget
    {
        [JsonProperty("intent")]
        public GitHubCheckRunTargetIntent Intent { get; set; }
        [JsonProperty("installationId")]
        public long InstallationId { get; set; }
        [JsonProperty("repositoryId")]
        public long RepositoryId { get; set; }
        [JsonProperty("sha")]
        public string Sha { get; set; }

        public override string ToString()
        {
            return $"{InstallationId}/{RepositoryId}/{Sha}";

        }
    }
}
