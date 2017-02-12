using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API.Mappings;

namespace GitClientVS.Infrastructure.Mappings
{
    public static class BitBucketEnterpriseMappings
    {
        public static void AddEnterpriseProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<EnterpriseToStandardMappingsProfile>();
        }
    }
}
