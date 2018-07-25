using System;
using System.Collections.Generic;
using System.Text;

namespace OpenInWeb
{
    public class VstsWebProvider : IWebProvider
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

        public string CreateFileUrl(string repositoryUrl, string relativePath, string version, ISelectionInformation selection)
        {
            var sb = new UriBuilder(repositoryUrl);

            var query = new Dictionary<string, string>
            {
                ["path"]            = $"/{PathHelpers.ToUnixPath(relativePath)}",
                ["version"]         = $"GB{version}",
                ["lineStyle"]       = "plain",
                ["line"]            = selection.StartLineNumber.ToString(),
                ["lineEnd"]         = selection.EndLineNumber.ToString(),
                ["lineStartColumn"] = selection.StartCharacterNumber.ToString(),
                ["lineEndColumn"]   = selection.EndCharacterNumber.ToString()
            };

            sb.Query = UriHelper.ToQueryString(query);

            return sb.ToString();
        }
    }
}
