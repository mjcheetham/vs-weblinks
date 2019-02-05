using System;
using System.Text.RegularExpressions;

namespace Mjcheetham.WebLinks
{
    internal class GitHubWebProvider : IWebProvider
    {
        public string Name => "GitHub";
        private string SshUriPattern => @"(.+)@(?'host'.+):(?'organization'[^\/]+)\/(?'repo'[^\/]+)\/?";

        public bool CanHandle(string repositoryUrl)
        {
            return IsSshUri(repositoryUrl) || IsHttpUri(repositoryUrl);
        }

        public string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection)
        {
            var baseUrl = ExtractBaseFileUrl(repositoryUrl);
            var sb = new UriBuilder(baseUrl);

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

        private bool IsSshUri(string repositoryUrl)
        {
            Match match = Regex.Match(repositoryUrl, SshUriPattern);

            if (match.Success)
            {
                string hostName = match.Groups["host"].Value;
                return StringComparer.OrdinalIgnoreCase.Equals(hostName, "github.com");
            }
            return false;
        }

        private bool IsHttpUri(string repositoryUrl)
        {
            try
            {
                var uri = new Uri(repositoryUrl);
                return uri.Host.Equals("github.com");
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
        private string ExtractBaseFileUrl(string repositoryUrl)
        {
            if (IsSshUri(repositoryUrl))
            {
                Match match = Regex.Match(repositoryUrl, SshUriPattern);
                string organization = match.Groups["organization"].Value;
                string repo = match.Groups["repo"].Value;

                return $"https://github.com/{organization}/{repo}";
            }
            else
            {
                if (repositoryUrl.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                {
                    return repositoryUrl.Substring(0, repositoryUrl.Length - ".git".Length);
                }
                return repositoryUrl;
            }
        }
    }
}
