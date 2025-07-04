﻿using BuildingBlocks.Core.EFCore;
using BuildingBlocks.Web.MinimalApi;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicStore.Modules.Customers.Data;
using MusicStore.Modules.Customers.Events.Integration;

namespace MusicStore.Modules.Customers;
public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgresDbContext<CustomersDbContext>(configuration);

        services.AddValidatorsFromAssembly(typeof(CustomersModule).Assembly, includeInternalTypes: true);
        services.AddMinimalApiEndpoints(typeof(CustomersModule).Assembly);

        return services;
    }

    public static IApplicationBuilder UseCustomersModule(this IApplicationBuilder app)
    {
        app.UseMigration<CustomersDbContext>();

        return app;
    }
    
    public static IRegistrationConfigurator AddCustomersModuleConsumers(this IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<UserRegisteredConsumer>();
        configurator.AddConsumer<OrderStatusChangedToAwaitingValidationConsumer>();
        
        return configurator;
    }
}
