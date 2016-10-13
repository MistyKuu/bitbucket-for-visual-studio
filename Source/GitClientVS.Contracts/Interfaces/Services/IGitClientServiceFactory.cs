using System.Collections.Generic;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitClientServiceFactory
    {
        IGitClientService GetService();
        IEnumerable<GitProviderType> AvailableServices { get; }
        IGitClientService GetService(GitProviderType gitProvider);
    }
}