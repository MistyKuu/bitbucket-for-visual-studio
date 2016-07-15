using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IBitbucketService
    {
        Task ConnectAsync(string login, string password);
    }
}
