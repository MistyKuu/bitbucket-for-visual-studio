using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BitBucket.REST.API.Interfaces
{
    public interface IProxyResolver
    {
        ICredentials Authenticate(string proxyUrl);
        ICredentials GetCredentials();
    }
}
