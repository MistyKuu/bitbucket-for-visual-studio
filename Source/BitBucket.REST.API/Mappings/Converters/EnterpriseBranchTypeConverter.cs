using AutoMapper;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Mappings.Converters
{
    public class EnterpriseBranchTypeConverter : ITypeConverter<EnterpriseBranch, Branch>
    {
        public Branch Convert(EnterpriseBranch source, Branch destination, ResolutionContext context)
        {
            return new Branch()
            {
                Name = source.DisplayId,
                Target = new Commit()
                {
                    Hash = source.LatestCommitId
                },
            };
        }
    }
}