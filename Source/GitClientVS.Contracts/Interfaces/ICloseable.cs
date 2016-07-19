using System;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ICloseable
    {
        event EventHandler Closed;
    }
}