using System;
using System.Collections.Generic;

namespace Mjcheetham.WebLinks
{
    internal class AzureDevOpsWebProvider : IWebProvider
    {
        public string Name => "Azure DevOps";

        public bool CanHandle(string repositoryUrl)
        {
            var uri = new Uri(repositoryUrl);
            string hostName = uri.Host;

            if (StringComparer.OrdinalIgnoreCase.Equals(hostName, "dev.azure.com"))
            {
                return true;
            }

            if (hostName.EndsWith(".visualstudio.com", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection)
        {
            var sb = new UriBuilder(repositoryUrl);

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
    }
}
