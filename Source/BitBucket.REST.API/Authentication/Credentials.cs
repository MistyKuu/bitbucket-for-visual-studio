using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API.Authentication;

namespace BitBucket.REST.API.Models
{
    public class Credentials
    {
        public Credentials()
        {
            AuthenticationType = AuthenticationType.Anonymous;
        }

        public Credentials(string token)
        {
            Login = null;
            Password = token;
            AuthenticationType = AuthenticationType.OAuth;
        }

        public Credentials(string login, string password) : this(login, password,login)
        {
        }

        public Credentials(string login, string password, string accountId)
        {
            Login = login;
            Password = password;
            AccountId = accountId;
            AuthenticationType = AuthenticationType.Basic;
        }

        public string Login { get; private set; }
        public string AccountId { get; private set; }

        public string Password { get; private set; }

        public AuthenticationType AuthenticationType { get; private set; }
    }
}
