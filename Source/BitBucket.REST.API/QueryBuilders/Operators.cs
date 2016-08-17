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
}