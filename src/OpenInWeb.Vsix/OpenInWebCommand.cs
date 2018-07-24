using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
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
            IOpenInWebService openInWebService = _package.GetService<IOpenInWebService>();
            if (openInWebService != null)
            {
                IWpfTextViewHost currentViewHost = EditorHelper.GetCurrentViewHost(_package);
                if (currentViewHost != null)
                {
                    ITextDocument document = EditorHelper.GetTextDocumentForView(currentViewHost);
                    ITextSelection selection = EditorHelper.GetSelection(currentViewHost);

                    var start = EditorHelper.GetLineAndColumnFromPosition(selection.Start.Position);
                    var end = EditorHelper.GetLineAndColumnFromPosition(selection.End.Position);

                    string filePath = document?.FilePath;

                    var selectionInfo = new VsEditorSelectionInfo(start.line, start.character, end.line, end.character);

                    openInWebService.OpenSelectionInWeb(filePath, selectionInfo);
                }
            }
        }

        private class VsEditorSelectionInfo : ISelectionInformation
        {
            public int StartLineNumber      { get; }
            public int StartCharacterNumber { get; }
            public int EndLineNumber        { get; }
            public int EndCharacterNumber   { get; }

            public VsEditorSelectionInfo(int startLine, int startChar, int endLine, int endChar)
            {
                StartLineNumber      = startLine;
                StartCharacterNumber = startChar;
                EndLineNumber        = endLine;
                EndCharacterNumber   = endChar;
            }
        }
    }
}
