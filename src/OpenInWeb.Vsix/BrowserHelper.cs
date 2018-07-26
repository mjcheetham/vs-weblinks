using System.Diagnostics;

namespace OpenInWeb.Vsix
{
    internal static class BrowserHelper
    {
        public static void OpenBrowser(string url)
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
