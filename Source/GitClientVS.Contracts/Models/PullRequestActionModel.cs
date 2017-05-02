using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace GitClientVS.Contracts.Models
{
    public class PullRequestActionModel : ReactiveObject
    {
        public string Label { get; }
        public ICommand Command { get; }

        public PullRequestActionModel(string label, ReactiveCommand<Unit, Unit> command)
        {
            Label = label;
            Command = command;
        }
    }
}
