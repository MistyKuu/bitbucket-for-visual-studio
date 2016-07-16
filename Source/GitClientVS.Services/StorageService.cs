using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure;
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

        public Result SaveUserData(ConnectionData connectionData) // TODO USE NIBA SERIALIZER
        {
            try
            {
                var serializedJson = JsonConvert.SerializeObject(connectionData);
                var hashedCredentials = _hashService.Encrypt(serializedJson);
                _fileService.Save(Paths.GitClientUserDataPath, hashedCredentials);
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
                var creds = _fileService.Read(Paths.GitClientUserDataPath);
                var unecrypted = _hashService.Decrypt(creds);
                var connectionData = JsonConvert.DeserializeObject<ConnectionData>(unecrypted);
                return Result<ConnectionData>.Success(connectionData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Result<ConnectionData>.Fail(ex);
            }
        }
    }
}
