using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitClientVS.Contracts.Events
{
    public class ShowConfirmationEvent
    {
        public ICommand Command { get; }
        public string Title { get; }
        public string Message { get; }


        public ShowConfirmationEvent(string title, string message, ICommand command)
        {
            Command = command;
            Title = title;
            Message = message;
        }
    }
}
