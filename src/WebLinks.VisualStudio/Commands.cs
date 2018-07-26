using System;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Mjcheetham.WebLinks.VisualStudio
{
    internal sealed class CopyFileLinkCommand : VsCommand
    {
        public const int CommandId = 0x0100;

        private readonly IServiceProvider _serviceProvider;
        private readonly IWebLinksService _webLinksService;

        public CopyFileLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webLinksService = _serviceProvider.GetService<IWebLinksService>();
        }

        protected override void QueryStatusInternal(OleMenuCommand command)
        {
            command.Enabled = false;
            command.Visible = false;

            if (_webLinksService != null)
            {
                string filePath = SelectionHelper.GetSelectedFilePath(_serviceProvider);

                string providerName = _webLinksService.GetProviderForFile(filePath);
                if (!string.IsNullOrWhiteSpace(providerName))
                {
                    command.Enabled = true;
                    command.Visible = true;
                    command.Text = string.Format(Resources.CopyWebLink_Command_ProviderFormat, providerName);
                }
            }
        }

        protected override void InvokeInternal()
        {
            if (_webLinksService != null)
            {
                string filePath = SelectionHelper.GetSelectedFilePath(_serviceProvider);
                string url = _webLinksService.GetFileUrl(filePath);
                Clipboard.SetText(url);
            }
        }
    }

    internal sealed class OpenFileLinkCommand : VsCommand
    {
        public const int CommandId = 0x0101;

        private readonly IServiceProvider _serviceProvider;
        private readonly IWebLinksService _webLinksService;

        public OpenFileLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webLinksService = _serviceProvider.GetService<IWebLinksService>();
        }

        protected override void QueryStatusInternal(OleMenuCommand command)
        {
            command.Enabled = false;
            command.Visible = false;

            if (_webLinksService != null)
            {
                string filePath = SelectionHelper.GetSelectedFilePath(_serviceProvider);

                string providerName = _webLinksService.GetProviderForFile(filePath);
                if (!string.IsNullOrWhiteSpace(providerName))
                {
                    command.Enabled = true;
                    command.Visible = true;
                    command.Text = string.Format(Resources.OpenInWeb_Command_ProviderFormat, providerName);
                }
            }
        }

        protected override void InvokeInternal()
        {
            if (_webLinksService != null)
            {
                string filePath = SelectionHelper.GetSelectedFilePath(_serviceProvider);
                string url = _webLinksService.GetFileUrl(filePath);
                BrowserHelper.OpenBrowser(url);
            }
        }
    }

    internal sealed class CopySelectionLinkCommand : VsCommand
    {
        public const int CommandId = 0x0110;

        private readonly IServiceProvider _serviceProvider;
        private readonly IWebLinksService _webLinksService;

        public CopySelectionLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webLinksService = _serviceProvider.GetService<IWebLinksService>();
        }

        protected override void QueryStatusInternal(OleMenuCommand command)
        {
            command.Enabled = false;
            command.Visible = false;

            if (_webLinksService != null)
            {
                var selection = SelectionHelper.GetEditorSelection(_serviceProvider);

                string providerName = _webLinksService.GetProviderForFile(selection?.FilePath);
                if (!string.IsNullOrWhiteSpace(providerName))
                {
                    command.Enabled = true;
                    command.Visible = true;
                    command.Text = string.Format(Resources.CopyWebLink_Command_ProviderFormat, providerName);
                }
            }
        }

        protected override void InvokeInternal()
        {
            if (_webLinksService != null)
            {
                var selection = SelectionHelper.GetEditorSelection(_serviceProvider);

                string url = _webLinksService.GetFileSelectionUrl(
                    selection.FilePath,
                    selection.StartLine,
                    selection.EndLine,
                    selection.StartCharacter,
                    selection.EndCharacter);

                Clipboard.SetText(url);
            }
        }
    }

    internal sealed class OpenSelectionLinkCommand : VsCommand
    {
        public const int CommandId = 0x0111;

        private readonly IServiceProvider _serviceProvider;
        private readonly IWebLinksService _webLinksService;

        public OpenSelectionLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webLinksService = _serviceProvider.GetService<IWebLinksService>();
        }

        protected override void QueryStatusInternal(OleMenuCommand command)
        {
            command.Enabled = false;
            command.Visible = false;

            if (_webLinksService != null)
            {
                var selection = SelectionHelper.GetEditorSelection(_serviceProvider);

                string providerName = _webLinksService.GetProviderForFile(selection?.FilePath);
                if (!string.IsNullOrWhiteSpace(providerName))
                {
                    command.Enabled = true;
                    command.Visible = true;
                    command.Text = string.Format(Resources.OpenInWeb_Command_ProviderFormat, providerName);
                }
            }
        }

        protected override void InvokeInternal()
        {
            if (_webLinksService != null)
            {
                var selection = SelectionHelper.GetEditorSelection(_serviceProvider);

                string url = _webLinksService.GetFileSelectionUrl(
                    selection.FilePath,
                    selection.StartLine,
                    selection.EndLine,
                    selection.StartCharacter,
                    selection.EndCharacter);

                BrowserHelper.OpenBrowser(url);
            }
        }
    }
}
