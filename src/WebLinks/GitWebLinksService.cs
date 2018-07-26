using System.Collections.Generic;

namespace Mjcheetham.WebLinks
{
    public class GitWebLinksService : IWebLinksService
    {
        private readonly ICollection<IWebProvider> _providers = new IWebProvider[]
        {
            new GitHubWebProvider(),
            new VstsWebProvider(),
        };

        #region IWebLinksService

        public string GetProviderForFile(string filePath)
        {
            if (_providers.Count == 0)
            {
                return null;
            }

            string repositoryPath = Git.GetRepositoryPath(filePath);
            string repositoryUrl = Git.GetRepositoryUrl(repositoryPath);

            foreach (IWebProvider webProvider in _providers)
            {
                if (webProvider.CanHandle(repositoryUrl))
                {
                    return webProvider.Name;
                }
            }

            return null;
        }

        public string GetFileUrl(string filePath)
        {
            return GetFileUrlFromProvider(filePath, null, null);
        }

        public string GetFileSelectionUrl(string filePath, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            if (_providers.Count == 0)
            {
                return null;
            }

            string repositoryPath = Git.GetRepositoryPath(filePath);
            string versionBranch  = Git.GetCurrentRepositoryVersion(repositoryPath, resolveRef: true);
            string versionCommit  = Git.GetCurrentRepositoryVersion(repositoryPath, resolveRef: false);

            var version = new VersionInformation(versionBranch, versionCommit);
            var selection = new SelectionInformation(lineStart, lineEnd, charStart, charEnd);

            return GetFileUrlFromProvider(filePath, version, selection);
        }

        #endregion

        #region Private methods

        private string GetFileUrlFromProvider(string filePath, VersionInformation version, SelectionInformation selection)
        {
            string repositoryPath = Git.GetRepositoryPath(filePath);
            string repositoryUrl = Git.GetRepositoryUrl(repositoryPath);
            string relativePath = PathHelpers.GetRelativePath(repositoryPath, filePath);

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

    #endregion
}
