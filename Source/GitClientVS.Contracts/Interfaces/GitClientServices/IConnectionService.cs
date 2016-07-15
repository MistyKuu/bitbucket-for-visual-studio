using GitClientVS.Contracts.Models.GitClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces.GitClientServices
{
    public interface IConnectionService
    {
        void Connect(GitClientHostAddress hostAddress);


    }
}
