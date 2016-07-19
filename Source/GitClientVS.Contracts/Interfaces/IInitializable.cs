using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IInitializable<in T>
    {
        Task InitializeAsync(T param);
    }

    public interface IInitializable
    {
        Task InitializeAsync();
    }
}
