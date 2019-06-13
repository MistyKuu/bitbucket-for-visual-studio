using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IUserInformationService : IDisposable
    {
        List<ConnectionData> GetSavedUsers();
        ConnectionData ConnectionData { get; }
        Theme CurrentTheme { get; }
        void StartListening();
    }
}