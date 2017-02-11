using System;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.QueryBuilders
{
    public interface IQueryParam
    {
        IQueryConnector UpdatedOn(DateTime date, Operators queryOperator);
        IQueryConnector CreatedOn(DateTime date, Operators queryOperator);
        IQueryConnector State(PullRequestOptions option);
    }
}