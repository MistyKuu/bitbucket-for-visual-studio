using System.Collections.Generic;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class PageIterator<T>
    {
        public int Page { get; set; }
        public string Next { get; set; }
        public List<T> Values { get; set; }

        public bool HasNext()
        {
            return Next != null;
        }
    }
}