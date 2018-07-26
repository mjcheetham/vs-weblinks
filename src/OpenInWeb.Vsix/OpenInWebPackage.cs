using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace OpenInWeb.Vsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageId)]
    public sealed class OpenInWebPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            this.AddServiceAsync<IWebLinksService>(CreateOpenInWebService, false);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Register commands
            OleMenuCommandService commandService = await this.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();

            RegisterCommand(commandService, Constants.MainCommandSet, OpenSelectionLinkCommand.CommandId, new OpenSelectionLinkCommand(this));
            RegisterCommand(commandService, Constants.MainCommandSet, CopySelectionLinkCommand.CommandId, new CopySelectionLinkCommand(this));
        }

        private static Task<IWebLinksService> CreateOpenInWebService()
        {
            return Task.FromResult<IWebLinksService>(new GitWebLinksService());
        }

        private static void RegisterCommand(OleMenuCommandService commandService, Guid commandSet, int commandId, ICommand command)
        {
            var menuCommandId = new CommandID(commandSet, commandId);
            var menuItem = new MenuCommand((s, e) => command.Execute(null), menuCommandId);
            commandService.AddCommand(menuItem);
        }
    }
}
