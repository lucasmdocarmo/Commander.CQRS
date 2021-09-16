using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommander<T>(this IServiceCollection services) where T : Message
        {
            services.AddScoped<ICommander, Commander>();
            services.AddCommanderHandlers(typeof(T));
            services.AddCommanderValidation(typeof(T));
        }

        private static void AddCommanderHandlers(this IServiceCollection services, Type type) => type.Assembly
            .GetTypes()
            .Where(type => type.GetInterfaces().Any(CheckHandlers)).ToList()
            .ForEach(type => type.GetInterfaces().Where(CheckHandlers).ToList().ForEach(@interface => services.TryAddScoped(@interface, type)));

        private static void AddCommanderValidation(this IServiceCollection services, Type type) => type.Assembly
            .GetTypes()
            .Where(type => type.BaseType.Is(typeof(CommanderValidator<>))).ToList()
            .ForEach(type => services.TryAddScoped(type.BaseType, type));

        private static bool CheckHandlers(Type type) => type.Is(typeof(ICommandHandler<,>)) || type.Is(typeof(ICommandHandler<>)) || type.Is(typeof(IEventHandler<>))
            || type.Is(typeof(IQueryHandler<,>)) || type.Is(typeof(IQueryHandler<>));
    }
}
