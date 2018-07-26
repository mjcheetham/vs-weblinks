using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Mjcheetham.WebLinks.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageId)]
    public sealed class WebLinksPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            this.AddServiceAsync<IWebLinksService>(CreateOpenInWebService, false);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Register commands
            var commandManager = new CommandManager(this);

            commandManager.RegisterCommand(Constants.MainCommandSet, OpenSelectionLinkCommand.CommandId, new OpenSelectionLinkCommand(this));
            commandManager.RegisterCommand(Constants.MainCommandSet, CopySelectionLinkCommand.CommandId, new CopySelectionLinkCommand(this));
            commandManager.RegisterCommand(Constants.MainCommandSet, OpenFileLinkCommand.CommandId,      new OpenFileLinkCommand(this));
            commandManager.RegisterCommand(Constants.MainCommandSet, CopyFileLinkCommand.CommandId,      new CopyFileLinkCommand(this));
        }

        private static Task<IWebLinksService> CreateOpenInWebService()
        {
            return Task.FromResult<IWebLinksService>(new GitWebLinksService());
        }
    }
}
