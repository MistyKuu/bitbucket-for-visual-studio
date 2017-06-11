namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IDataNotifier
    {
        bool ShouldUpdate { get; set; }
    }
}