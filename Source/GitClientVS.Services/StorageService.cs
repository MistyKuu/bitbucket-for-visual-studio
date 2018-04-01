using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API.Serializers;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Extensions;
using log4net;
using Newtonsoft.Json;

namespace GitClientVS.Services
{
    [Export(typeof(IStorageService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StorageService : IStorageService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFileService _fileService;
        private readonly IHashService _hashService;

        [ImportingConstructor]
        public StorageService(IFileService fileService, IHashService hashService)
        {
            _fileService = fileService;
            _hashService = hashService;
        }

        public Result SaveUserData(ConnectionData connectionData) => SaveEncryptedSettings(connectionData, Paths.GitClientUserDataPath);
        public Result<ConnectionData> LoadUserData() => LoadEncryptedSettings<ConnectionData>(Paths.GitClientUserDataPath);
        public Result SaveProxySettings(ProxySettings proxySettings) => SaveEncryptedSettings(proxySettings, Paths.GitClientProxyDataPath);
        public Result<ProxySettings> LoadProxySettings() => LoadEncryptedSettings<ProxySettings>(Paths.GitClientProxyDataPath);


        private Result SaveEncryptedSettings<T>(T settings, string filePath)
        {
            try
            {
                JsonConvert.SerializeObject(settings)
                              .Then(_hashService.Encrypt)
                              .Then(cred => _fileService.Save(filePath, cred));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result.Fail(ex);
            }

            return Result.Success();
        }

        private Result<T> LoadEncryptedSettings<T>(string filePath) where T : class
        {
            try
            {
                if (!File.Exists(filePath))
                    return Result<T>.Fail();

                return _fileService
                    .Read(filePath)
                    .Then(_hashService.Decrypt)
                    .Then(JsonConvert.DeserializeObject<T>)
                    .Then(Result<T>.Success);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result<T>.Fail(ex);
            }
        }

    }
}
