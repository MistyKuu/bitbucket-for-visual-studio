using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }
        public static TBase GetService<TChild, TBase>(this IServiceProvider serviceProvider) where TChild : class where TBase : class
        {
            return serviceProvider.GetService(typeof(TChild)) as TBase;
        }
    }
}