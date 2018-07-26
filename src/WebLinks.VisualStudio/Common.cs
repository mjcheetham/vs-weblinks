using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Mjcheetham.WebLinks.VisualStudio
{
    public static class Common
    {
        public static string GetEditorSelectionFileUrl(IServiceProvider serviceProvider)
        {
            IWebLinksService webLinksService = serviceProvider.GetService<IWebLinksService>();
            if (webLinksService != null)
            {
                IWpfTextViewHost currentViewHost = EditorHelper.GetCurrentViewHost(serviceProvider);
                if (currentViewHost != null)
                {
                    ITextDocument document = EditorHelper.GetTextDocumentForView(currentViewHost);
                    ITextSelection selection = EditorHelper.GetSelection(currentViewHost);

                    var start = EditorHelper.GetLineAndColumnFromPosition(selection.Start.Position);
                    var end = EditorHelper.GetLineAndColumnFromPosition(selection.End.Position);

                    string filePath = document?.FilePath;

                    return webLinksService.GetFileSelectionUrl(filePath, start.line, end.line, start.character, end.character);
                }
            }

            return null;
        }

        public static string GetSelectedFileUrl(IServiceProvider serviceProvider)
        {
            IWebLinksService webLinksService = serviceProvider.GetService<IWebLinksService>();
            if (webLinksService != null)
            {
                IVsMonitorSelection monitorSelection = serviceProvider.GetService<SVsShellMonitorSelection, IVsMonitorSelection>();

                monitorSelection.GetCurrentSelection(out IntPtr hierarchyPtr, out uint itemId, out _, out _);
                if (hierarchyPtr != IntPtr.Zero)
                {
                    IVsHierarchy hierarchy = (IVsHierarchy)Marshal.GetUniqueObjectForIUnknown(hierarchyPtr);

                    hierarchy.GetCanonicalName(itemId, out string filePath);
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        // The canonical name might not match the case of the file on disk
                        filePath = PathHelpers.GetCorrectlyCasedPath(filePath);

                        return webLinksService.GetFileUrl(filePath);
                    }
                }
            }

            return null;
        }
    }
}
