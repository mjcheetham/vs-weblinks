using System.Collections.Generic;

namespace OpenInWeb
{
    public class GitWebLinksService : IWebLinksService
    {
        private readonly ICollection<IWebProvider> _providers = new IWebProvider[]
        {
            new GitHubWebProvider(),
            new VstsWebProvider(),
        };

        public string GetFileSelectionUrl(string filePath, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            if (_providers.Count == 0)
            {
                return null;
            }

            string repositoryPath = Git.GetRepositoryPath(filePath);
            string repositoryUrl  = Git.GetRepositoryUrl(repositoryPath);
            string relativePath   = PathHelpers.GetRelativePath(repositoryPath, filePath);
            string versionBranch  = Git.GetCurrentRepositoryVersion(repositoryPath, resolveRef: true);
            string versionCommit  = Git.GetCurrentRepositoryVersion(repositoryPath, resolveRef: false);

            var version = new VersionInformation
            {
                BranchName = versionBranch,
                CommitId = versionCommit
            };

            var selection = new SelectionInformation
            {
                StartLineNumber      = lineStart,
                EndLineNumber        = lineEnd,
                StartCharacterNumber = charStart,
                EndCharacterNumber   = charEnd,
            };

            foreach (IWebProvider webProvider in _providers)
            {
                if (webProvider.CanHandle(repositoryUrl))
                {
                    string url = webProvider.CreateFileUrl(repositoryUrl, relativePath, version, selection);
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        return url;
                    }
                }
            }

            return null;
        }
    }
}
