using System;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IUserInformationService : IDisposable
    {
        ConnectionData ConnectionData { get; }
        Theme CurrentTheme { get; }
        void StartListening();
    }
}