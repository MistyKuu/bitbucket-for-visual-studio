using System;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IUserInformationService
    {
        ConnectionData ConnectionData { get; }
    }
}