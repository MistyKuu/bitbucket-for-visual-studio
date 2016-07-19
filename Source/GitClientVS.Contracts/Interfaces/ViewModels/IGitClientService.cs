using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IGitClientService
    {
        string Title { get; }
        Task LoginAsync(string login, string password);
        void Logout();
    }
}
