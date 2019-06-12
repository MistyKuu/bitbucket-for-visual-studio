﻿using System.Globalization;
using AutoMapper;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseCommitTypeConverter : ITypeConverter<EnterpriseCommit, Commit>
    {
        public Commit Convert(EnterpriseCommit source, Commit destination, ResolutionContext context)
        {
            var commit = new Commit()
            {
                Author = new Author()
                {
                    User = source.Author.MapTo<User>(),
                },
                Hash = source.Id,
                Date = source.Date.FromUnixTimeStamp().ToString(CultureInfo.InvariantCulture),
                Message = source.Message
            };
            return commit;
        }
    }
}