using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace Mjcheetham.WebLinks.VisualStudio
{
    internal class CommandManager
    {
        private readonly OleMenuCommandService _menuService;

        public CommandManager(IServiceProvider serviceProvider)
        {
            _menuService = serviceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
        }

        public void RegisterCommand(Guid commandSet, int commandId, VsCommand command)
        {
            AddCommand(commandSet, commandId, command.Invoke, command.QueryStatus);
        }

        public void AddCommand(Guid commandGroupGuid, int commandId, EventHandler invokeHandler, EventHandler beforeQueryStatus)
        {
            OleMenuCommand command = new OleMenuCommand(
                invokeHandler,
                delegate { },
                beforeQueryStatus,
                new CommandID(commandGroupGuid, commandId));

            _menuService.AddCommand(command);
        }
    }

    internal abstract class VsCommand
    {
        protected virtual void QueryStatusInternal(OleMenuCommand command)
        {
        }

        protected abstract void InvokeInternal();

        public void Invoke(object sender, EventArgs args)
        {
            if (sender is OleMenuCommand command && command.Enabled)
            {
                this.InvokeInternal();
            }
        }

        public void QueryStatus(object sender, EventArgs args)
        {
            this.QueryStatusInternal(sender as OleMenuCommand);
        }
    }
}
