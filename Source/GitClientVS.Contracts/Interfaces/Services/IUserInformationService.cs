using System;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IUserInformationService : IDisposable
    {
        ConnectionData ConnectionData { get; }
        void LoadStoreInformation();
    }
}