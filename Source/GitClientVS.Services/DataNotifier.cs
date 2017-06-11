using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;

namespace GitClientVS.Services
{
    [Export(typeof(IDataNotifier))]
    [PartCreationPolicy(CreationPolicy.Shared)] //todo not very elegant solution to pass the information about state change. Make it better later
    public class DataNotifier : IDataNotifier
    {
        public bool ShouldUpdate { get; set; }
    }
}
