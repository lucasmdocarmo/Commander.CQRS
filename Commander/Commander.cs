using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Commander
{
    public sealed class Commander : ICommander
    {
        private readonly IServiceProvider _serviceProvider;

        public Commander(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async ValueTask<ICommandResult<TResponse>> Execute<TRequest, TResponse>(TRequest request) where TRequest : ICommand
        {
            var validator = await ValidateRequestAsync(request);

            if (!validator.IsSuccess)
            {
                return CommandResult<TResponse>.IsFailed(validator.Message);
            }

            var service = _serviceProvider.GetService<ICommandHandler<TRequest, TResponse>>();

            if (service == default)
            {
                throw new ArgumentException($"Class implementing {nameof(ICommandHandler<TRequest, TResponse>)} not found or not implemented!");
            }

            return await service.Execute(request).ConfigureAwait(false);
        }

        public async ValueTask<ICommandResult> Execute<TRequest>(TRequest request) where TRequest : ICommand
        {
            var validator = await ValidateRequestAsync(request);

            if (!validator.IsSuccess)
            {
                return CommandResult.IsFailed(validator.Message);
            }

            var service = _serviceProvider.GetService<ICommandHandler<TRequest>>();

            if (service == default)
            {
                throw new ArgumentException($"Class implementing {nameof(ICommandHandler<TRequest>)} not found or not implemented!");
            }

            return await service.Execute(request).ConfigureAwait(false);
        }

        public async ValueTask Publish<TRequest>(TRequest notification) where TRequest : IEvent
        {
            var services = _serviceProvider.GetServices<IEventHandler<TRequest>>();

            if (!services.Any())
            {
                throw new ArgumentException($"No class implementing {nameof(IEventHandler<TRequest>)} not found or not implemented!");
            }

            await Task.WhenAll(services.Select(service => service.Publish(notification).AsTask())).ConfigureAwait(false);
        }

        private async Task<ICommandResult> ValidateRequestAsync<TRequest>(TRequest request) where TRequest : ICommand
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
