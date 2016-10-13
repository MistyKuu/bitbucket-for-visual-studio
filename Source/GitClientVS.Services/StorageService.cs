using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        public Result SaveUserData(ConnectionData connectionData) 
        {
            try
            {
                JsonConvert.SerializeObject(connectionData)
                   .Then(_hashService.Encrypt)
                   .Then(cred => _fileService.Save(Paths.GitClientUserDataPath, cred));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result.Fail(ex);
            }

            return Result.Success();
        }

        public Result<ConnectionData> LoadUserData()
        {
            try
            {
                return _fileService
                    .Read(Paths.GitClientUserDataPath)
                    .Then(_hashService.Decrypt)
                    .Then(JsonConvert.DeserializeObject<ConnectionData>)
                    .Then(Result<ConnectionData>.Success);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result<ConnectionData>.Fail(ex);
            }
        }
    }
}
