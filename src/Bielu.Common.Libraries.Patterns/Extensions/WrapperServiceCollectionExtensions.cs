// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Wrapper;
using Microsoft.Extensions.DependencyInjection;

namespace Bielu.Common.Libraries.Patterns.Extensions;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> for registering
/// <see cref="IWrapper{TService}"/> implementations in the dependency injection container.
/// </summary>
public static class WrapperServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a transient <see cref="IWrapper{TService}"/>.
    /// The wrapper is resolved from the container and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddTransientWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : class, IWrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IWrapper<TService>, TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a scoped <see cref="IWrapper{TService}"/>.
    /// The wrapper is resolved from the container and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddScopedWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : class, IWrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IWrapper<TService>, TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers <typeparamref name="TWrapper"/> as a singleton <see cref="IWrapper{TService}"/>.
    /// The wrapper is resolved from the container and receives the inner service via constructor injection.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <typeparam name="TWrapper">The wrapper implementation type.</typeparam>
    public static IServiceCollection AddSingletonWrapper<TService, TWrapper>(this IServiceCollection services)
        where TService : class
        where TWrapper : class, IWrapper<TService>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IWrapper<TService>, TWrapper>();
        return services;
    }

    /// <summary>
    /// Registers a transient <see cref="IWrapper{TService}"/> using a factory delegate.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddTransientWrapper<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, IWrapper<TService>> wrapperFactory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddTransient<IWrapper<TService>>(wrapperFactory);
        return services;
    }

    /// <summary>
    /// Registers a scoped <see cref="IWrapper{TService}"/> using a factory delegate.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddScopedWrapper<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, IWrapper<TService>> wrapperFactory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddScoped<IWrapper<TService>>(wrapperFactory);
        return services;
    }

    /// <summary>
    /// Registers a singleton <see cref="IWrapper{TService}"/> using a factory delegate.
    /// </summary>
    /// <typeparam name="TService">The service type being wrapped.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="wrapperFactory">A factory that receives the <see cref="IServiceProvider"/>
    /// and returns the wrapper instance.</param>
    public static IServiceCollection AddSingletonWrapper<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, IWrapper<TService>> wrapperFactory)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(wrapperFactory);

        services.AddSingleton<IWrapper<TService>>(wrapperFactory);
        return services;
    }
}
