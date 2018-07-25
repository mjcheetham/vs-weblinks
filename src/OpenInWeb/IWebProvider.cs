namespace OpenInWeb
{
    public interface IWebProvider
    {
        bool CanHandle(string repositoryUrl);

        string CreateFileUrl(string repositoryUrl, string relativePath, string version, ISelectionInformation selection);
    }
}
