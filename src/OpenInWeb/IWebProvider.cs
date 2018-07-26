namespace OpenInWeb
{
    internal interface IWebProvider
    {
        bool CanHandle(string repositoryUrl);

        string CreateFileUrl(string repositoryUrl, string relativePath, VersionInformation version, SelectionInformation selection);
    }

    internal struct SelectionInformation
    {
        public int StartLineNumber;

        public int StartCharacterNumber;

        public int EndLineNumber;

        public int EndCharacterNumber;
    }

    internal struct VersionInformation
    {
        public string BranchName;

        public string CommitId;
    }
}
