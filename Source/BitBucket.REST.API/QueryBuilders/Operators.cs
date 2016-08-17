using System.Collections.Generic;
using System.ComponentModel;

namespace BitBucket.REST.API.QueryBuilders
{
    public enum Operators
    {
        [Description(">")]
        Greater,
        [Description("<")]
        Lesser
    }

    static class OperatorsMappings
    {
        static readonly Dictionary<Operators, string> MappingsDictionary = new Dictionary<Operators, string>()
        {
            {Operators.Greater, ">"},
            {Operators.Lesser, "<" }
        };
        
    }
}