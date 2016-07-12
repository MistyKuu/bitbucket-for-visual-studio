using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitbucketVS.Infrastructure.ViewModels;
using BitBucketVs.Contracts.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace BitbucketVS.Infrastructure
{
    public class ContainerInitializer
    {
        public void Initialize()
        {
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(ConfigureUnityContainer()));
        }

        private static IUnityContainer ConfigureUnityContainer()
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IMainViewModel, MainViewModel>();
            return container;
        }
    }
}
