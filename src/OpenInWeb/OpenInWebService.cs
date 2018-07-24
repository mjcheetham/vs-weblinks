using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenInWeb
{
    public interface IOpenInWebService
    {
        void OpenSelectionInWeb(string filePath, ISelectionInformation selection);
    }

    public class OpenInWebService : IOpenInWebService
    {
        private readonly ICollection<IWebProvider> _providers;

        public OpenInWebService(IEnumerable<IWebProvider> providers)
        {
            _providers = providers.ToList();
        }

        public void OpenSelectionInWeb(string filePath, ISelectionInformation selection)
        {
            if (_providers.Count == 0)
            {
                return;
            }

            string repositoryPath = Git.GetRepositoryPath(filePath);
            string repositoryUrl  = Git.GetRepositoryUrl(repositoryPath);
            string relativePath   = PathHelpers.GetRelativePath(repositoryPath, filePath);
            string version        = Git.GetCurrentRepositoryVersion(repositoryPath);

            foreach (IWebProvider webProvider in _providers)
            {
                string url = webProvider.CreateFileUrl(repositoryUrl, relativePath, version, selection);
                if (!string.IsNullOrWhiteSpace(url))
                {
                    OpenBrowser(url);
                }
            }
        }

        private void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                // Squash all errors!
            }
        }
    }
}
