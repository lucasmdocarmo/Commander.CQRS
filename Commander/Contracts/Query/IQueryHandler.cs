using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface IQueryHandler<in TRequest, TResponse> where TRequest : Query
    {
        ValueTask<IQueryResult<TResponse>> ExecuteQuery(TRequest request);
    }
    public interface IQueryHandler<in TRequest> where TRequest : Query
    {
        ValueTask<IQueryResult> ExecuteQuery(TRequest request);
    }

}
