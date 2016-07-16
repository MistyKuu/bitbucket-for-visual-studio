using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Infrastructure.Events
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
