namespace Mjcheetham.WebLinks
{
    public interface IWebLinksService
    {
        string GetProviderForFile(string filePath);

        string GetFileUrl(string filePath);

        string GetFileSelectionUrl(string filePath, int lineStart, int lineEnd, int charStart, int charEnd);
    }
}
