using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Mjcheetham.WebLinks.VisualStudio
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

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        #endregion

        protected virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    internal sealed class CopySelectionLinkCommand : SelectionLinkCommand
    {
        public const int CommandId = 0x0110;

        public CopySelectionLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetSelectedTextUrl();
            Clipboard.SetText(url);
        }
    }

    internal sealed class OpenSelectionLinkCommand : SelectionLinkCommand
    {
        public const int CommandId = 0x0111;

        public OpenSelectionLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetSelectedTextUrl();
            BrowserHelper.OpenBrowser(url);
        }
    }
}
