using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace OpenInWeb.Vsix
{
    internal abstract class SelectionLinkCommand : ICommand
    {
        protected readonly AsyncPackage Package;

        protected SelectionLinkCommand(AsyncPackage package)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
        }

        protected string GetSelectedTextUrl()
        {
            IWebLinksService webLinksService = Package.GetService<IWebLinksService>();
            if (webLinksService != null)
            {
                IWpfTextViewHost currentViewHost = EditorHelper.GetCurrentViewHost(Package);
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

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        #endregion

    }

    internal sealed class OpenSelectionLinkCommand : SelectionLinkCommand
    {
        public const int CommandId = 0x0100;

        public OpenSelectionLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetSelectedTextUrl();
            BrowserHelper.OpenBrowser(url);
        }
    }

    internal sealed class CopySelectionLinkCommand : SelectionLinkCommand
    {
        public const int CommandId = 0x0101;

        public CopySelectionLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetSelectedTextUrl();
            Clipboard.SetText(url);
        }
    }
}
