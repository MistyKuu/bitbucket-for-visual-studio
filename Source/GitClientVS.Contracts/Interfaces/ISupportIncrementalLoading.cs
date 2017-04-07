using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ISupportIncrementalLoading
    {
        Task LoadNextPageAsync();
    }
}