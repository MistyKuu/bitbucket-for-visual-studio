using System.Collections.Generic;
using AutoMapper;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET;

namespace GitClientVS.Infrastructure.Mappings.Gitlab
{
    public class GitlabPaginatedConverter<TSource, TDest> : ITypeConverter<PaginatedResult<TSource>, PageIterator<TDest>>
    {
        public PageIterator<TDest> Convert(PaginatedResult<TSource> source, PageIterator<TDest> destination, ResolutionContext context)
        {
            return new PageIterator<TDest>()
            {
                Next = source.NextPage.ToString(),
                Page = (int)source.CurrentPage.Value,
                Values = source.Data.MapTo<List<TDest>>()
            };
        }
    }
}