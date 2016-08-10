using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IDiffFileParser
    {
        IEnumerable<FileDiff> Parse(string diff);
    }
}