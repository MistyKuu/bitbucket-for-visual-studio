using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Mappings;
using log4net;

namespace GitClientVS.VisualStudio.UI
{
    [Export(typeof(IAppInitializer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppInitializer : IAppInitializer
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IStorageService _storageService;
        private readonly IGitClientService _gitClient;
        private readonly IUserInformationService _userInformationService;

        [ImportingConstructor]
        public AppInitializer(
            IStorageService storageService,
            IGitClientService gitClient,
            IUserInformationService userInformationService
            )
        {
            _storageService = storageService;
            _gitClient = gitClient;
            _userInformationService = userInformationService;
        }

        public async Task Initialize()
        {
            try
            {
                _userInformationService.StartListening();
                var result = _storageService.LoadUserData();

                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<GitMappingsProfile>();
                    BitBucketEnterpriseMappings.AddEnterpriseProfile(cfg);
                });

                await GitClientLogin(result);
            }
            catch (Exception e)
            {
                Logger.Error("Error during App initialization: " + e);
            }
        }

        private async Task GitClientLogin(Result<CombinedConnectionData> result)
        {
            if (result.IsSuccess && result.Data.Current.IsLoggedIn)
            {
                try
                {
                    var cred = new GitCredentials()
                    {
                        Login = result.Data.Current.UserName,
                        Password = result.Data.Current.Password,
                        Host = result.Data.Current.Host,
                        IsEnterprise = result.Data.Current.IsEnterprise
                    };
                    await _gitClient.LoginAsync(cred);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Couldn't login user using stored credentials. Credentials must have been changed or there is no internet connection", ex);
                }
            }
        }
    }
}
