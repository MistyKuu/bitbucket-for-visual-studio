using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;

namespace GitClientVS.Services
{
    [Export(typeof(IEventAggregatorService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EventAggregatorService : IEventAggregatorService
    {
        private readonly EventAggregator _eventAggregator;

        public EventAggregatorService()
        {
            _eventAggregator = new EventAggregator();
        }

        public void Dispose()
        {
            _eventAggregator.Dispose();
        }

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return _eventAggregator.GetEvent<TEvent>();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            _eventAggregator.Publish(sampleEvent);
        }
    }

    public class EventAggregator
    {
        readonly Subject<object> subject = new Subject<object>();

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            subject.OnNext(sampleEvent);
        }

        bool disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            subject.Dispose();

            disposed = true;
        }
    }
}
