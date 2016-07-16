using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Events
{
    public class ConnectionChangedEvent
    {
        public bool IsLoggedIn { get; set; }
    }
}
