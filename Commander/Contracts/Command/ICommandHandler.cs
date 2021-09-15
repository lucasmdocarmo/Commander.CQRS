using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface ICommandHandler<in TRequest, TResponse> where TRequest : ICommand
    {
        ValueTask<ICommandResult<TResponse>> Execute(TRequest request);
    }

    public interface ICommandHandler<in TRequest> where TRequest : ICommand
    {
        ValueTask<ICommandResult> Execute(TRequest request);
    }
}
