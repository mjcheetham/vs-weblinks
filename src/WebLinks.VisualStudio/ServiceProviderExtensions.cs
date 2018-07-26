using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Mjcheetham.WebLinks.VisualStudio
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

        public static void AddServiceAsync<T>(this IAsyncServiceContainer container, Func<Task<T>> createCallback, bool promote)
        {
            container.AddService(typeof(T), async (c, ct, t) => await createCallback(), promote);
        }

        public static void AddService<T>(this IServiceContainer container, T instance, bool promote)
        {
            container.AddService(typeof(T), instance, promote);
        }
    }
}
