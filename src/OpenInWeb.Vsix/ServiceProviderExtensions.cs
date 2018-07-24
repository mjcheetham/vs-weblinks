using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace OpenInWeb.Vsix
{
    internal static class ServiceProviderExtensions
    {
        public static Task<T> GetServiceAsync<T>(this IAsyncServiceProvider sp)
        {
            return GetServiceAsync<T, T>(sp);
        }

        public static async Task<TInterface> GetServiceAsync<TService, TInterface>(this IAsyncServiceProvider sp)
        {
            return (TInterface)await sp.GetServiceAsync(typeof(TService));
        }

        public static T GetService<T>(this IServiceProvider sp)
        {
            return GetService<T, T>(sp);
        }

        public static TInterface GetService<TService, TInterface>(this IServiceProvider sp)
        {
            return (TInterface)sp.GetService(typeof(TService));
        }
    }
}
