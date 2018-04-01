using BitBucket.REST.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.Composition;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Interfaces.ViewModels;

namespace GitClientVS.Services
{
    [Export(typeof(IProxyResolver))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoginProxyResolver : IProxyResolver
    {
        private readonly ExportFactory<IProxyLoginDialogView> _proxyViewFactory;
        private readonly IStorageService _storageService;

        [ImportingConstructor]
        public LoginProxyResolver(ExportFactory<IProxyLoginDialogView> proxyViewFactory, IStorageService storageService)
        {
            _proxyViewFactory = proxyViewFactory;
            _storageService = storageService;
        }

        public ICredentials GetCredentials()
        {
            var result = _storageService.LoadProxySettings();

            if (result.IsSuccess)
                return new NetworkCredential(result.Data.Name, result.Data.Password);

            return null;
        }

        public ICredentials Authenticate(string proxyUrl)
        {
            var proxyDialogView = _proxyViewFactory.CreateExport().Value;
            var vm = proxyDialogView.DataContext as IProxyLoginDialogViewModel;
            vm.ProxyUrl = proxyUrl;

            var res = proxyDialogView.ShowDialog();
            if (res.Value)
            {
                _storageService.SaveProxySettings(new Contracts.Models.ProxySettings() { Name = vm.Login, Password = vm.Password });
                return new NetworkCredential(vm.Login, vm.Password);
            }

            return null;
        }
    }
}
