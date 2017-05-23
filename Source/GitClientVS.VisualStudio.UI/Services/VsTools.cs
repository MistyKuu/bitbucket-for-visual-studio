using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using GitClientVS.Contracts.Interfaces.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IVsTools))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VsTools : IVsTools
    {
        private readonly IAppServiceProvider _appServiceProvider;

        [ImportingConstructor]
        public VsTools(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }
    }
}
