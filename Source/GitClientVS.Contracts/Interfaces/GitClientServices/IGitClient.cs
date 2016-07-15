using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces.GitClientServices
{
    public interface IGitClient
    {

        IConnectionService ConnectionService { get; }
        IRepositoriesService RepositoriesService { get; }

    }
}
