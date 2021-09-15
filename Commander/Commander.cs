using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Contracts.Result;
using Microsoft.Extensions.DependencyInjection;

namespace Commander
{
    public sealed class Commander : ICommander
    {
        private readonly IServiceProvider _serviceProvider;

        public Commander(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async ValueTask<ICommandResult<TResponse>> Execute<TRequest, TResponse>(TRequest request) where TRequest : Command
        {
            var validator = await ValidateRequestAsync(request);

            if (!validator.IsSuccess)
            {
                return CommandResult<TResponse>.IsFailed(validator.Message);
            }

            var service = _serviceProvider.GetService<ICommandHandler<TRequest, TResponse>>();

            if (service == default)
            {
                throw new CommandException($"Command {nameof(ICommandHandler<TRequest, TResponse>)} not found or not implemented!");
            }
            try
            {
                return await service.Execute(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new CommandException($"An Error ocourred while executing command {request?.GetType().Name} in handler {service?.GetType().Name}", ex.InnerException);
            }
        }

        public async ValueTask<ICommandResult> Execute<TRequest>(TRequest request) where TRequest : Command
        {
            var validator = await ValidateRequestAsync(request);

            if (!validator.IsSuccess)
            {
                return CommandResult.IsFailed(validator.Message);
            }

            var service = _serviceProvider.GetService<ICommandHandler<TRequest>>();

            if (service == default)
            {
                throw new CommandException($"Command {nameof(ICommandHandler<TRequest>)} not found or not implemented!");
            }

            try
            {
                return await service.Execute(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new CommandException($"An Error ocourred while exectuingg command {nameof(ICommandHandler<TRequest>)}", ex.InnerException);
            }
        }

        public async ValueTask<IEventResult> Publish<TRequest>(TRequest notification) where TRequest : Event
        {
            var services = _serviceProvider.GetServices<IEventHandler<TRequest>>();

            if (!services.Any())
            {
                throw new CommandException($"Event {nameof(IEventHandler<TRequest>)} not found or not implemented!");
            }
            try
            {
                await Task.WhenAll(services.Select(service => service.Publish(notification).AsTask())).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw new EventException($"An Error ocourred while posting event  {nameof(IEventHandler<TRequest>)}",ex.InnerException);
            }

            return await ValueTask.FromResult(new EventResult());
        }

        private async Task<ICommandResult> ValidateRequestAsync<TRequest>(TRequest request) where TRequest : Command
        {
            var validator = _serviceProvider.GetService<CommanderValidator<TRequest>>();

            if (validator == default)
            {
                return await CommandResult.SuccessAsync();
            }

            return await validator.ValidateAsync(request);
        }
    }
}
