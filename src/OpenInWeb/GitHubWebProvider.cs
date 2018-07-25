using System;

namespace OpenInWeb
{
    public class GitHubWebProvider : IWebProvider
    {
        public bool CanHandle(string repositoryUrl)
        {
            var uri = new Uri(repositoryUrl);

            return uri.Host.Equals("github.com");
        }

        public string CreateFileUrl(string repositoryUrl, string relativePath, string version, ISelectionInformation selection)
        {
            var sb = new UriBuilder(repositoryUrl);

            UriHelper.AppendPath(sb, $"blob/{version ?? "master"}");
            UriHelper.AppendPath(sb, PathHelpers.ToUnixPath(relativePath));

            // GitHub only supports linking to a single line rather than a span
            // so just use the starting line of the selection.
            sb.Fragment = $"L{selection.StartLineNumber}";

            return sb.ToString();
        }
    }
}
