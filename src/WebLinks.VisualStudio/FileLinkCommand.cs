using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Mjcheetham.WebLinks.VisualStudio
{
    internal abstract class FileLinkCommand : ICommand
    {
        protected readonly AsyncPackage Package;

        protected FileLinkCommand(AsyncPackage package)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
        }

        protected string GetFileUrl()
        {
            IWebLinksService webLinksService = Package.GetService<IWebLinksService>();
            if (webLinksService != null)
            {
                IVsMonitorSelection monitorSelection = Package.GetService<SVsShellMonitorSelection, IVsMonitorSelection>();

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

    internal sealed class CopyFileLinkCommand : FileLinkCommand
    {
        public const int CommandId = 0x0100;

        public CopyFileLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetFileUrl();
            Clipboard.SetText(url);
        }
    }

    internal sealed class OpenFileLinkCommand : FileLinkCommand
    {
        public const int CommandId = 0x0101;

        public OpenFileLinkCommand(AsyncPackage package) : base(package) { }

        public override void Execute(object parameter)
        {
            string url = GetFileUrl();
            BrowserHelper.OpenBrowser(url);
        }
    }
}
