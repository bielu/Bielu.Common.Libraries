// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Wrapper;
using Microsoft.Extensions.DependencyInjection;

namespace Bielu.Common.Libraries.Patterns.Extensions;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> for registering
/// <see cref="Wrapper{TService}"/> implementations in the dependency injection container.
/// Wrappers are registered as their concrete type, unlike decorators which use a service interface.
/// </summary>
public static class WrapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a transient service.
    /// The wrapper is resolved from the container by its concrete type
    /// and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddTransientWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : Wrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a scoped service.
    /// The wrapper is resolved from the container by its concrete type
    /// and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddScopedWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : Wrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a singleton service.
    /// The wrapper is resolved from the container by its concrete type
    /// and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddSingletonWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : Wrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers a transient wrapper using a factory delegate.
    /// The wrapper is resolved from the container by its concrete type.
    /// </summary>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddTransientWrapper<TWrapper>(
        this IServiceCollection services,
        Func<IServiceProvider, TWrapper> wrapperFactory)
        where TWrapper : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddTransient(wrapperFactory);
        return services;
    }

    /// <summary>
    /// Registers a scoped wrapper using a factory delegate.
    /// The wrapper is resolved from the container by its concrete type.
    /// </summary>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddScopedWrapper<TWrapper>(
        this IServiceCollection services,
        Func<IServiceProvider, TWrapper> wrapperFactory)
        where TWrapper : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddScoped(wrapperFactory);
        return services;
    }

    /// <summary>
    /// Registers a singleton wrapper using a factory delegate.
    /// The wrapper is resolved from the container by its concrete type.
    /// </summary>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddSingletonWrapper<TWrapper>(
        this IServiceCollection services,
        Func<IServiceProvider, TWrapper> wrapperFactory)
        where TWrapper : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddSingleton(wrapperFactory);
        return services;
    }
}
