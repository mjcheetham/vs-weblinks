using System;

namespace Mjcheetham.WebLinks
{
    internal class GitHubWebProvider : IWebProvider
    {
        public string Name => "GitHub";

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
                bool isSpanSelection = selection.StartLineNumber != selection.EndLineNumber;

                // GitHub only supports linking to a single lines or line ranges; it doesn't
                // support column or character selections.
                if (isSpanSelection)
                {
                    sb.Fragment = $"L{selection.StartLineNumber}-L{selection.EndLineNumber}";
                }
                else
                {
                    sb.Fragment = $"L{selection.StartLineNumber}";
                }
            }

            return sb.ToString();
        }
    }
}
