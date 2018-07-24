namespace OpenInWeb
{
    public interface IWebProvider
    {
        string CreateFileUrl(string repositoryUrl, string relativePath, string version, ISelectionInformation selection);
    }
}
