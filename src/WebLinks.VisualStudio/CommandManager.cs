using System;
using System.ComponentModel.Design;
using System.Diagnostics;
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
                try
                {
                    this.InvokeInternal();
                }
                catch (Exception ex)
                {
                    // Swallow exception to prevent VS crash
                    Trace.WriteLine($"Unhandled exception in VsCommand.Invoke:{Environment.NewLine}{ex}");
                }
            }
        }

        public void QueryStatus(object sender, EventArgs args)
        {
            try
            {
                this.QueryStatusInternal(sender as OleMenuCommand);
            }
            catch (Exception ex)
            {
                // Swallow exception to prevent VS crash
                Trace.WriteLine($"Unhandled exception in VsCommand.QueryStatus:{Environment.NewLine}{ex}");
            }
        }
    }
}
