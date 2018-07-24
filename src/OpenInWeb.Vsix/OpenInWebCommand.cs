using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Task = System.Threading.Tasks.Task;

namespace OpenInWeb.Vsix
{
    internal sealed class OpenInWebCommand
    {
        public const int CommandId = 0x0100;

        private readonly AsyncPackage _package;

        private OpenInWebCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(Constants.MainCommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static OpenInWebCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new OpenInWebCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            GetSelectedTextInfo();
        }

        private void GetSelectedTextInfo()
        {
            IWpfTextViewHost currentViewHost = EditorHelper.GetCurrentViewHost(_package);

            if (currentViewHost != null)
            {
                ITextDocument document = EditorHelper.GetTextDocumentForView(currentViewHost);
                ITextSelection selection = EditorHelper.GetSelection(currentViewHost);

                VirtualSnapshotPoint start = selection.Start;
                VirtualSnapshotPoint end = selection.End;

                int startLineNumber = start.Position.Snapshot.GetLineNumberFromPosition(start.Position);
                int endLineNumber   = end.Position.Snapshot.GetLineNumberFromPosition(end.Position);

                string filePath = document?.FilePath;

                VsShellUtilities.ShowMessageBox(
                    this._package,
                    $"Selection from line {startLineNumber} to {endLineNumber}",
                    "Open In Web",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
