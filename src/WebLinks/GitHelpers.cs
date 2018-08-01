using System;
using LibGit2Sharp;

namespace Mjcheetham.WebLinks
{
    internal static class GitHelpers
    {
        public static string GetRepositoryUrl(string repositoryPath)
        {
            if (Repository.IsValid(repositoryPath))
            {
                using (var repository = new Repository(repositoryPath))
                {
                    var originRemote = repository.Network.Remotes["origin"];
                    if (originRemote != null)
                    {
                        return originRemote.Url;
                    }
                }
            }

            return null;
        }

        public static string GetCurrentRepositoryVersion(string repositoryPath, bool resolveRef)
        {
            if (Repository.IsValid(repositoryPath))
            {
                using (var repository = new Repository(repositoryPath))
                {
                    return resolveRef
                         ? repository.Head.FriendlyName
                         : repository.Head.Tip.Id.Sha;
                }
            }

            return null;
        }

        public static string GetRepositoryPath(string filePath)
        {
            const string gitDirPathPart = ".git\\";
            string raw = Repository.Discover(filePath);

            if (raw.EndsWith(gitDirPathPart, StringComparison.OrdinalIgnoreCase))
            {
                return raw.Substring(0, raw.Length - gitDirPathPart.Length);
            }

            return raw;
        }
    }
}
