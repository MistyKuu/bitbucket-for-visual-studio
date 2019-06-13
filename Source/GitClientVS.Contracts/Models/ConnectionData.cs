using System;
using System.Collections.Generic;
using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace GitClientVS.Contracts.Models
{
    public class ConnectionData
    {
        [JsonProperty]
        public string Id { get; set; }
        [JsonProperty]
        public bool IsLoggedIn { get; set; }
        [JsonProperty]
        public bool IsLoggingIn { get; set; }
        [JsonProperty]
        public string UserName { get; set; }
        [JsonProperty]
        public string Password { get; set; }
        [JsonProperty]
        public Uri Host { get; set; }
        [JsonProperty]
        public bool IsEnterprise { get; set; }

        public static ConnectionData NotLogged => new ConnectionData() { IsLoggedIn = false };


        protected bool Equals(ConnectionData other)
        {
            return string.Equals(Id, other.Id) && Equals(Host, other.Host);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConnectionData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Host != null ? Host.GetHashCode() : 0);
            }
        }
    }

    public class CombinedConnectionData
    {
        [JsonProperty]
        public ConnectionData Current { get; set; }

        [JsonProperty]
        public List<ConnectionData> SavedUsers { get; set; }
    }
}