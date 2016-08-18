using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Events;
using GitClientVS.Infrastructure.Mappings;
using GitClientVS.UI.Helpers;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.VisualStudio.UI
{
    [Export(typeof(IAppInitializer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppInitializer : IAppInitializer
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IStorageService _storageService;
        private readonly IGitClientService _gitClient;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly ICacheService _cacheService;

        [ImportingConstructor]
        public AppInitializer(
            IStorageService storageService,
            IGitClientService gitClient,
            IEventAggregatorService eventAggregator,
            ICacheService cacheService
            )
        {
            _storageService = storageService;
            _gitClient = gitClient;
            _eventAggregator = eventAggregator;
            _cacheService = cacheService;
            _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>().Subscribe(_=> cacheService.Delete(CacheKeys.PullRequestCacheKey));
        }

        public async Task Initialize()
        {
            LoggerConfigurator.Setup();
            var result = _storageService.LoadUserData();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<GitMappingsProfile>();
            });

            await GitClientLogin(result);
        }

        private async Task GitClientLogin(Result<ConnectionData> result)
        {
            if (result.IsSuccess && result.Data.IsLoggedIn)
            {
                try
                {
                    await _gitClient.LoginAsync(result.Data.UserName, result.Data.Password);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Couldn't login user using stored credentials. Credentials must have been changed or there is no internet connection", ex);
                }
            }
        }
    }
}
