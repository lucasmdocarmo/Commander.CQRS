﻿using Microsoft.Extensions.DependencyInjection;
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
        private static bool IsRequestHandlers(Type type) => type.Is(typeof(IRequestHandler<,>)) || type.Is(typeof(IRequestHandler<>)) || type.Is(typeof(IEventHandler<>));

        public static void AddCommander(this IServiceCollection services, Type type)
        {
            services.AddScoped<ICommander, Commander>();
            services.AddRequestHandlers(type);
            services.AddValidators(type);
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