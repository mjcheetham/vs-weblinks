using System;

namespace Mjcheetham.WebLinks
{
    internal class GitHubWebProvider : IWebProvider
    {
        public bool CanHandle(string repositoryUrl)
        {
            var uri = new Uri(repositoryUrl);

            return uri.Host.Equals("github.com");
        }

        public string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection)
        {
            var sb = new UriBuilder(repositoryUrl);

            string versionString;
            if (version?.BranchName != null)
            {
                versionString = $"blob/{version.BranchName}";
            }
            else if (version?.CommitId != null)
            {
                versionString = $"blob/{version.CommitId}";
            }
            else
            {
                versionString = "blob/master";
            }

            UriHelper.AppendPath(sb, versionString);
            UriHelper.AppendPath(sb, PathHelpers.ToUnixPath(relativePath));

            if (selection != null)
            {
                // GitHub only supports linking to a single line rather than a span
                // so just use the starting line of the selection.
                sb.Fragment = $"L{selection.StartLineNumber}";
            }

            return sb.ToString();
        }
    }
}
