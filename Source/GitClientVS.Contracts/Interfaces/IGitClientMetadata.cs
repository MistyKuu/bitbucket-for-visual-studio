using GitClientVS.Contracts;

namespace GitClientVS.Services
{
    public interface IGitClientMetadata
    {
        GitProviderType GitProvider { get; }
    }
}