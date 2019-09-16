﻿using Microsoft.Build.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Octokit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Azure.Sdk.Tools.CheckEnforcer
{
    public class ConfigurationStore
    {
        public ConfigurationStore(GitHubClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        private GitHubClientFactory clientFactory;

        public async Task<RepositoryConfiguration> GetRepositoryConfigurationAsync(long installationId, long repositoryId, CancellationToken cancellationToken)
        {
            var client = await clientFactory.GetInstallationClientAsync(installationId, cancellationToken);
            var searchResults = await client.Repository.Content.GetAllContents(repositoryId, "CHECKENFORCER");
            var configurationFile = searchResults.SingleOrDefault();

            if (configurationFile == null) return new RepositoryConfiguration()
            {
                IsEnabled = false
            };

            ThrowIfInvalidFormat(configurationFile);

            var builder = new DeserializerBuilder().Build();
            var configuration = builder.Deserialize<RepositoryConfiguration>(configurationFile.Content);
            return configuration;
        }

        private static void ThrowIfInvalidFormat(RepositoryContent configurationFile)
        {
            // TODO: This is gross. I want to look more closely at the YamlDotNet API
            //       to see if there is a way I can parse this file once and then do
            //       deserialization of a document. At the moment I'm parsing the string
            //       twice. I suspect that I'm just not grokking the API properly yet.

            var stream = new StringReader(configurationFile.Content);
            var yaml = new YamlStream();
            yaml.Load(stream);

            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
            var formatScalar = (YamlScalarNode)mapping.Children[new YamlScalarNode("format")];

            if (formatScalar.Value != "v0.1-alpha")
            {
                throw new CheckEnforcerConfigurationException("The value for the 'format' was not valid. Try v0.1-alpha.");
            }
        }
    }
}
