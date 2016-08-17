using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using ParseDiff;

namespace GitClientVS.Services
{
    [Export(typeof(IDiffFileParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MemoryCacheService : IMemoryCacheService
    {
        //public IEnumerable<FileDiff> Parse(string diff)
        //{
            
        //}
    }

    public interface IMemoryCacheService
    {
    }
}
