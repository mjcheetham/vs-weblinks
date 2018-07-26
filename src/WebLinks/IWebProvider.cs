namespace Mjcheetham.WebLinks
{
    internal interface IWebProvider
    {
        bool CanHandle(string repositoryUrl);

        string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection);
    }

    internal class SelectionInformation
    {
        public SelectionInformation(int startLine, int endLine, int startChar, int endChar)
        {
            StartLineNumber = startLine;
            EndLineNumber = endLine;
            StartCharacterNumber = startChar;
            EndCharacterNumber = endChar;
        }

        public int StartLineNumber { get; }

        public int EndLineNumber { get; }

        public int StartCharacterNumber { get; }

        public int EndCharacterNumber { get; }
    }

    internal class VersionInformation
    {
        public VersionInformation(string branchName, string commitId)
        {
            BranchName = branchName;
            CommitId = commitId;
        }

        public string BranchName { get; }

        public string CommitId { get; }
    }
}
