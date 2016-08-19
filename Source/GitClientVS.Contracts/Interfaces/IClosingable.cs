using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IClosingable
    {
        event EventHandler Closing;
    }
}
