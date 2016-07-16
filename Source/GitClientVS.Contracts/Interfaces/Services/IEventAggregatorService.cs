using System;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IEventAggregatorService
    {
        void Dispose();
        IObservable<TEvent> GetEvent<TEvent>();
        void Publish<TEvent>(TEvent sampleEvent);
    }
}
