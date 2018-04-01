using System;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ICloseable<TResult>
    {
        event EventHandler<TResult> Closed;
    }

    public interface ICloseable
    {
        event EventHandler Closed;
    }
}