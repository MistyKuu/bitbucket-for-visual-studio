using System;
using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces.Views
{
    public interface IPullRequestsWindowContainerViewModel
    {
        event EventHandler Closed;
    }
}