using System.Collections.Generic;
using System.Diagnostics;

namespace Mjcheetham.WebLinks
{
    public class GitWebLinksService : IWebLinksService
    {
        private readonly ICollection<IWebProvider> _providers = new IWebProvider[]
        {
            new GitHubWebProvider(),
            new AzureDevOpsWebProvider(),
        };

        #region IWebLinksService

        public string GetProviderForFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            if (_providers.Count == 0)
            {
                return null;
            }

            string repositoryPath = GitHelpers.GetRepositoryPath(filePath);
            var trackingInfo = GitHelpers.GetTrackingInfoForHead(repositoryPath);

            if (trackingInfo == null)
            {
                // HEAD does not track any remote branch so there can be no
                // valid hosting provider.
                return null;
            }

            // Check all providers for one who can handle this remote URL
            foreach (IWebProvider webProvider in _providers)
            {
                if (webProvider.CanHandle(trackingInfo.RemoteUrl))
                {
                    return webProvider.Name;
                }
            }

            return null;
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            string repositoryPath = GitHelpers.GetRepositoryPath(filePath);
            var trackingInfo = GitHelpers.GetTrackingInfoForHead(repositoryPath);

            if (trackingInfo == null)
            {
                Debug.Fail("Should not be executing if there is no valid provider");
                return null;
            }

            var version = new VersionInformation(trackingInfo.BranchName, trackingInfo.CommitId);

            return GetFileUrlFromProvider(repositoryPath, trackingInfo.RemoteUrl, filePath, version, null);
        }

        public string GetFileSelectionUrl(string filePath, int lineStart, int lineEnd, int charStart, int charEnd)
        {
            if (_providers.Count == 0)
            {
                return null;
            }

            string repositoryPath = GitHelpers.GetRepositoryPath(filePath);
            var trackingInfo = GitHelpers.GetTrackingInfoForHead(repositoryPath);

            if (trackingInfo == null)
            {
                Debug.Fail("Should not be executing if there is no valid provider");
                return null;
            }

            var version = new VersionInformation(trackingInfo.BranchName, trackingInfo.CommitId);
            var selection = new SelectionInformation(lineStart, lineEnd, charStart, charEnd);

            return GetFileUrlFromProvider(repositoryPath, trackingInfo.RemoteUrl, filePath, version, selection);
        }

        #endregion

        #region Private methods

        private string GetFileUrlFromProvider(string repositoryPath, string remoteUrl, string filePath, VersionInformation version, SelectionInformation selection)
        {
            string relativePath = PathHelpers.GetRelativePath(repositoryPath, filePath);

            foreach (IWebProvider webProvider in _providers)
            {
                if (webProvider.CanHandle(remoteUrl))
                {
                    string url = webProvider.CreateFileUrl(remoteUrl, relativePath, version, selection);
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
