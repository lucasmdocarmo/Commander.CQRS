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
        private static bool IsRequestHandlers(Type type) => type.Is(typeof(ICommandHandler<,>)) || type.Is(typeof(ICommandHandler<>)) || type.Is(typeof(IEventHandler<>));

        public static void AddCommander<T>(this IServiceCollection services) where T: class
        {
            services.AddScoped<ICommander, Commander>();
            services.AddRequestHandlers(typeof(T));
            services.AddValidators(typeof(T));
        }

        private static void AddRequestHandlers(this IServiceCollection services, Type type) => type.Assembly
            .GetTypes()
            .Where(type => type.GetInterfaces().Any(IsRequestHandlers)).ToList()
            .ForEach(type => type.GetInterfaces().Where(IsRequestHandlers).ToList().ForEach(@interface => services.TryAddScoped(@interface, type)));

        private static void AddValidators(this IServiceCollection services, Type type) => type.Assembly
            .GetTypes()
            .Where(type => type.BaseType.Is(typeof(CommanderValidator<>))).ToList()
            .ForEach(type => services.TryAddScoped(type.BaseType, type));
    }
}
