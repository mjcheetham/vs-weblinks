using System;
using System.Collections.Generic;

namespace Mjcheetham.WebLinks
{
    internal class VstsWebProvider : IWebProvider
    {
        public bool CanHandle(string repositoryUrl)
        {
            var uri = new Uri(repositoryUrl);

            if (uri.Host.EndsWith(".visualstudio.com", StringComparison.OrdinalIgnoreCase))
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
                ["path"]            = $"/{PathHelpers.ToUnixPath(relativePath)}",
                ["lineStyle"]       = "plain",
            };

            // Version
            if (version.BranchName != null)
            {
                query["version"] = $"GB{version.BranchName}";
            }
            else if (version.CommitId != null)
            {
                query["version"] = $"GC{version.CommitId}";
            }

            // Selection
            if (selection.StartLineNumber == selection.EndLineNumber &&
                selection.StartCharacterNumber == selection.EndCharacterNumber)
            {
                // Select the entire line if the user has no selection (just a caret located on the line)
                query["line"]            = selection.StartLineNumber.ToString();
            }
            else
            {
                query["line"]            = selection.StartLineNumber.ToString();
                query["lineEnd"]         = selection.EndLineNumber.ToString();
                query["lineStartColumn"] = selection.StartCharacterNumber.ToString();
                query["lineEndColumn"]   = selection.EndCharacterNumber.ToString();
            }

            sb.Query = UriHelper.ToQueryString(query);

            return sb.ToString();
        }
    }
}
