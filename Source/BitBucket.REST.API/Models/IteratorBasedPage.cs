using System.Collections.Generic;

namespace BitBucket.REST.API.Models
{
    public class IteratorBasedPage<T>
    {
        public int? pagelen { get; set; }
        public string next { get; set; }
        public List<T> values { get; set; }
        public ulong? size { get; set; }
    }
}