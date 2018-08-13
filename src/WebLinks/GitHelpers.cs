using System;
using LibGit2Sharp;

namespace Mjcheetham.WebLinks
{
    internal static class GitHelpers
    {
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

        public static TrackingInfo GetTrackingInfoForHead(string repositoryPath)
        {
            using (var repository = new Repository(repositoryPath))
            {
                Branch head = repository.Head;

                // Try and get the tracking branch info for the current HEAD
                string remoteName;
                string branchName = null;
                string commitId = null;
                if (head.IsTracking)
                {
                    remoteName = head.RemoteName;
                    branchName = head.TrackedBranch.FriendlyName.Substring(remoteName.Length + 1);

                    // If we're ahead of the remote branch then this commit doesn't exist
                    // there yet, so don't report a commit ID.
                    if (head.TrackingDetails.AheadBy == 0)
                    {
                        commitId = head.Tip.Id.Sha;
                    }
                }
                //else if (head.CanonicalName == "(no branch)") // Detached HEAD
                //{
                //    // Nothing we can do here? How to check if this commit exists on the server?
                //}
                else
                {
                    // If we don't have a branch tracking a specific remote, default to origin
                    // and no branch or commit info.
                    remoteName = "origin";
                }

                Remote remote = repository.Network.Remotes[remoteName];

                if (remote == null)
                {
                    // No remote available so there can be no valid hosting provider
                    return null;
                }
                else
                {
                    return new TrackingInfo
                    {
                        RemoteName = remote.Name,
                        RemoteUrl = remote.Url,
                        BranchName = branchName,
                        CommitId = commitId
                    };
                }
            }
        }

        public class TrackingInfo
        {
            public string RemoteName;
            public string RemoteUrl;
            public string BranchName;
            public string CommitId;
        }
    }
}
