﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.DataInitialization;
using Unmatched.Entities;
using Unmatched.Services;

namespace Unmatched.Registration;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IUnmatchedService, UnmatchedService>();
        services.AddTransient<IRatingCalculator, RatingCalculator>();
    }
    
    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
    
    public static void RegisterDataInitializers(this IServiceCollection services)
    {
        services.AddTransient<IDataInitializer<Hero>, HeroesDataInitializer>();
        services.AddTransient<IDataInitializer<Map>, MapsDataInitializer>();
        services.AddTransient<IDataInitializer<Player>, PlayersDataInitializer>();
        services.AddTransient<IDataInitializer<Sidekick>, SidekicksDataInitializer>();
        services.AddTransient<IDataInitializer<Tournament>, TournamentsDataInitializer>();
    }
}