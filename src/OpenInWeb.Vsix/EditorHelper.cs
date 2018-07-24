using System;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace OpenInWeb.Vsix
{
    internal static class EditorHelper
    {
        /// <summary>
        /// Get the currently active text editor host.
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <returns>Text editor host</returns>
        public static IWpfTextViewHost GetCurrentViewHost(IServiceProvider serviceProvider)
        {
            // http://msdn.microsoft.com/en-us/library/dd884850.aspx

            IVsTextManager textManager = serviceProvider.GetService<SVsTextManager,IVsTextManager>();
            textManager.GetActiveView(fMustHaveFocus: 1, pBuffer: null, ppView: out IVsTextView textView);

            if (textView is IVsUserData userData)
            {
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out object holder);

                return (IWpfTextViewHost) holder;
            }

            return null;
        }

        /// <summary>
        /// Get the <see cref="ITextDocument"/> for the given editor host.
        /// </summary>
        /// <param name="viewHost"></param>
        /// <returns></returns>
        public static ITextDocument GetTextDocumentForView(IWpfTextViewHost viewHost)
        {
            viewHost.TextView
                    .TextDataModel
                    .DocumentBuffer
                    .Properties
                    .TryGetProperty(typeof(ITextDocument), out ITextDocument document);
            return document;
        }

        /// <summary>
        /// Get the current selection for the given editor host.
        /// </summary>
        /// <param name="viewHost">Editor view host</param>
        /// <returns>Current selection</returns>
        public static ITextSelection GetSelection(IWpfTextViewHost viewHost)
        {
            return viewHost.TextView.Selection;
        }
    }
}
