using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Events
{
    public class ConnectionChangedEvent
    {
        public ConnectionData Data { get; }

        public ConnectionChangedEvent(ConnectionData data)
        {
            Data = data;
        }
    }
}
