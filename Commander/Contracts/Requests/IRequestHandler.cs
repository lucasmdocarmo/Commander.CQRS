using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest
    {
        ValueTask<ICommandResult<TResponse>> Execute(TRequest request);
    }

    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        ValueTask<ICommandResult> Execute(TRequest request);
    }
}
