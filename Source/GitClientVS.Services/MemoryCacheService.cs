using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using ParseDiff;

namespace GitClientVS.Services
{
    [Export(typeof(ICacheService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MemoryCacheService : ICacheService
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public void Add(string key, object value)
        {
            _cache.Add(key, value, DateTimeOffset.MaxValue);
        }
        public Result<T> Get<T>(string key) where T : class
        {
            try
            {
                return _cache.Contains(key) ? Result<T>.Success((T)_cache.Get(key)) : Result<T>.Fail();
            }
            catch (Exception ex)
            {
                return Result<T>.Fail(ex);
            }
        }

        public void Delete(string key)
        {
            if (_cache.Contains(key))
                _cache.Remove(key);
        }
    }
}
