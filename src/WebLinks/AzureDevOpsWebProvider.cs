using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mjcheetham.WebLinks
{
    internal class AzureDevOpsWebProvider : IWebProvider
    {
        public string Name => "Azure DevOps";
        private string SshUriPattern => @"(?:.+@)?(?'host'.+)\:v3\/(?'organization'[^\/]+)\/(?'project'[^\/]+)\/(?:_[^\/]+\/)?(?'repo'[^\/]+)\/?$";

        public bool CanHandle(string repositoryUrl)
        {
            return IsSshUri(repositoryUrl) || IsHttpUri(repositoryUrl);
        }

        public string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection)
        {
            var baseUrl = ExtractBaseFileUrl(repositoryUrl);
            var sb = new UriBuilder(baseUrl);

            var query = new Dictionary<string, string>
            {
                ["path"] = $"/{PathHelpers.ToUnixPath(relativePath)}",
            };

            // Version
            if (version != null)
            {
                if (version.BranchName != null)
                {
                    query["version"] = $"GB{version.BranchName}";
                }
                else if (version.CommitId != null)
                {
                    query["version"] = $"GC{version.CommitId}";
                }
            }

            // Selection
            if (selection != null)
            {
                bool isSpanSelection = !(selection.StartLineNumber == selection.EndLineNumber &&
                                         selection.StartCharacterNumber == selection.EndCharacterNumber);

                query["lineStyle"] = "plain";
                query["line"]      = selection.StartLineNumber.ToString();

                // If the user has no selection (just a caret location) then we should only
                // select the entire line and not a span.
                if (isSpanSelection)
                {
                    query["lineEnd"]         = selection.EndLineNumber.ToString();
                    query["lineStartColumn"] = selection.StartCharacterNumber.ToString();
                    query["lineEndColumn"]   = selection.EndCharacterNumber.ToString();
                }
            }

            sb.Query = UriHelper.ToQueryString(query);

            return sb.ToString();
        }

        private bool IsSshUri(string repositoryUrl)
        {
            Match match = Regex.Match(repositoryUrl, SshUriPattern);

            if (match.Success)
            {
                string hostName = match.Groups["host"].Value;
                return StringComparer.OrdinalIgnoreCase.Equals(hostName, "dev.azure.com")
                    || hostName.EndsWith(".visualstudio.com", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private bool IsHttpUri(string repositoryUrl)
        {
            var uri = new Uri(repositoryUrl);
            string hostName = uri.Host;

            return StringComparer.OrdinalIgnoreCase.Equals(hostName, "dev.azure.com")
                || hostName.EndsWith(".visualstudio.com", StringComparison.OrdinalIgnoreCase);
        }

        private string ExtractBaseFileUrl(string repositoryUrl)
        {
            if(IsSshUri(repositoryUrl))
            {
                Match match = Regex.Match(repositoryUrl, SshUriPattern);
                string organization = match.Groups["organization"].Value;
                string project = match.Groups["project"].Value;
                string repo = match.Groups["repo"].Value;

                return $"https://{organization}.visualstudio.com/DefaultCollection/{project}/_git/{repo}";
            }
            else
            {
                return repositoryUrl;
            }
        }
    }
}
