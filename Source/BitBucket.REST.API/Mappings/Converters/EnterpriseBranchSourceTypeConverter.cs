using AutoMapper;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseBranchSourceTypeConverter : ITypeConverter<EnterpriseBranchSource, Source>, ITypeConverter<Source, EnterpriseBranchSource>
    {
        public Source Convert(EnterpriseBranchSource source, Source destination, ResolutionContext context)
        {
            return new Source()
            {
                Repository = source.Repository.MapTo<Repository>(),
                Branch = new Branch() { Name = source.DisplayId },
                Commit = new Commit() { Hash = source.LatestCommitId }
            };
        }

        public EnterpriseBranchSource Convert(Source source, EnterpriseBranchSource destination, ResolutionContext context)
        {
            return new EnterpriseBranchSource()
            {
                Repository = source.Repository?.MapTo<EnterpriseRepository>(),
                DisplayId = source.Branch.Name,
                LatestCommitId = source.Commit?.Hash
            };
        }
    }
}