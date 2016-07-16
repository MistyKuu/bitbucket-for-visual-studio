using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using Reactive.EventAggregator;

namespace GitClientVS.Services
{
    [Export(typeof(IEventAggregatorService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class EventAggregatorService : IEventAggregatorService // TODO this is a wrapper for EventAggregator because registering an external object is sick in MEF
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
}
