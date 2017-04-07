using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BitBucket.REST.API.QueryBuilders
{
    public class QueryString : IEnumerable<KeyValuePair<string,string>>
    {
        private readonly Dictionary<string, string> _params = new Dictionary<string, string>();

        public override string ToString()
        {
            return "?" + String.Join("&", _params.Select(param => $"{param.Key}={param.Value}").ToArray());
        }

       
        public void Add(string key, string value)
        {
            _params.Add(key, value);
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _params.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
