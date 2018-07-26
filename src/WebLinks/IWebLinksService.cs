namespace Mjcheetham.WebLinks
{
    public interface IWebLinksService
    {
        string GetFileUrl(string filePath);

        string GetFileSelectionUrl(string filePath, int lineStart, int lineEnd, int charStart, int charEnd);
    }
}
