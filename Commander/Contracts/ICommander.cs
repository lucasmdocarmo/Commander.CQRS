using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public interface ICommander
    {
        /// <summary>
        /// Returns an IResult of TResult where request must be an IRequest.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        ValueTask<ICommandResult<TResponse>> Execute<TRequest, TResponse>(TRequest request) where TRequest : ICommand;

        /// <summary>
        /// Returns a Value Task. For use when don't require a return.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        ValueTask<ICommandResult> Execute<TRequest>(TRequest request) where TRequest : ICommand;

        /// <summary>
        /// Publish a Event for all handlers subscribed.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="notification"></param>
        /// <returns></returns>
        ValueTask Publish<TRequest>(TRequest notification) where TRequest : IEvent;
    }
}
