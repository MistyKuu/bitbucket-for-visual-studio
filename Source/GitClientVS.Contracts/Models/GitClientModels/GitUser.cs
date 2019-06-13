using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitUser
    {
        public string Uuid { get; set; }
        public string DisplayName { get; set; }
        public GitLinks Links { get; set; }

        protected bool Equals(GitUser other)
        {
            return string.Equals(Uuid, other.Uuid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GitUser) obj);
        }

        public override int GetHashCode()
        {
            return (Uuid != null ? Uuid.GetHashCode() : 0);
        }
    }
}
