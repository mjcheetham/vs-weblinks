using System;
using System.Windows;

namespace Mjcheetham.WebLinks.VisualStudio
{
    internal sealed class CopyFileLinkCommand : VsCommand
    {
        public const int CommandId = 0x0100;

        private readonly IServiceProvider _serviceProvider;

        public CopyFileLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void InvokeInternal()
        {
            string url = Common.GetSelectedFileUrl(_serviceProvider);
            Clipboard.SetText(url);
        }
    }

    internal sealed class OpenFileLinkCommand : VsCommand
    {
        public const int CommandId = 0x0101;

        private readonly IServiceProvider _serviceProvider;

        public OpenFileLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void InvokeInternal()
        {
            string url = Common.GetSelectedFileUrl(_serviceProvider);
            BrowserHelper.OpenBrowser(url);
        }
    }

    internal sealed class CopySelectionLinkCommand : VsCommand
    {
        public const int CommandId = 0x0110;

        private readonly IServiceProvider _serviceProvider;

        public CopySelectionLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void InvokeInternal()
        {
            string url = Common.GetEditorSelectionFileUrl(_serviceProvider);
            Clipboard.SetText(url);
        }
    }

    internal sealed class OpenSelectionLinkCommand : VsCommand
    {
        public const int CommandId = 0x0111;

        private readonly IServiceProvider _serviceProvider;

        public OpenSelectionLinkCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void InvokeInternal()
        {
            string url = Common.GetEditorSelectionFileUrl(_serviceProvider);
            BrowserHelper.OpenBrowser(url);
        }
    }
}
