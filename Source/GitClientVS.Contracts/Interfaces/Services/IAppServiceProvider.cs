using System;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IAppServiceProvider : IServiceProvider, IDisposable
    {
        IServiceProvider GitServiceProvider { get; set; }
        TService GetService<TService>() where TService : class;
        void AddService<TService>(TService obj, object owner);
    }
}