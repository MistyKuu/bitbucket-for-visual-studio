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

    internal static class OperatorsMappings
    {
        internal static readonly Dictionary<Operators, string> MappingsDictionary = new Dictionary<Operators, string>()
        {
            {Operators.Greater, ">"},
            {Operators.Lesser, "<" }
        };
        
    }
}