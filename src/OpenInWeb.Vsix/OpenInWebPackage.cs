using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
            this.AddServiceAsync<IOpenInWebService>(CreateOpenInWebService, false);

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await OpenInWebCommand.InitializeAsync(this);
        }

        private static Task<IOpenInWebService> CreateOpenInWebService()
        {
            var providers = new IWebProvider[]
            {
                new GitHubWebProvider(),
                new VstsWebProvider(),
            };

            var service = new OpenInWebService(providers);

            return Task.FromResult<IOpenInWebService>(service);
        }
    }
}
