using ReactiveUI;

namespace GitClientVS.Contracts.Models
{
    public class PullRequestActionModel : ReactiveObject
    {
        public string Label { get; }
        public ReactiveCommand Command { get; }

        public PullRequestActionModel(string label, ReactiveCommand command)
        {
            Label = label;
            Command = command;
        }
    }
}
