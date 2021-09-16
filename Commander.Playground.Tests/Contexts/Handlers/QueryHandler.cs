using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Playground.Tests;

namespace Commander.Playground.Tests.Contexts.Handlers
{
    public class QueryHandler : IQueryHandler<ProductQuery, ProductOutput>
    {
        public async ValueTask<IQueryResult<ProductOutput>> ExecuteQuery(ProductQuery request)
        {
            return await QueryResult<ProductOutput>.SuccessAsync(new ProductOutput());
        }
    }
}
