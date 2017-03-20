using ReactiveUI;

namespace GitClientVS.Contracts.Models
{
    public class PullRequestActionModel : ReactiveObject
    {
        public string Label { get; }
        public IReactiveCommand Command { get; }

        public PullRequestActionModel(string label, IReactiveCommand command)
        {
            Label = label;
            Command = command;
        }
    }
}
