using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;

namespace GitClientVS.Services
{
    [Export(typeof(IMessageBoxService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxService : IMessageBoxService
    {
        private readonly IEventAggregatorService _eventAggregator;

        [ImportingConstructor]
        public MessageBoxService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void ExecuteCommandWithConfirmation(string title, string message, ICommand command)
        {
            var ev = new ShowConfirmationEvent(title, message,command);
            _eventAggregator.Publish(ev);
        }
    }
}
