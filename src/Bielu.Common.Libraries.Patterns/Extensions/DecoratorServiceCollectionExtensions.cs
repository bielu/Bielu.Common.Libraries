// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Bielu.Common.Libraries.Patterns.Extensions;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> that allow registering Scrutor decorators
/// with an explicit priority/order. Lower priority numbers execute first (outermost decorator).
/// Call <see cref="ApplyDecoratorPriorities"/> after all registrations to apply them in the correct order.
/// </summary>
public static class DecoratorServiceCollectionExtensions
{
    /// <summary>
    /// Registers a decorator of type <typeparamref name="TDecorator"/> for service
    /// <typeparamref name="TService"/> with the specified priority.
    /// Lower priority numbers result in the decorator being the outermost wrapper
    /// (executed first). Call <see cref="ApplyDecoratorPriorities"/> to apply.
    /// </summary>
    public static IServiceCollection DecorateWithPriority<TService, TDecorator>(
        this IServiceCollection services,
        int priority)
        where TService : class
        where TDecorator : class, TService
    {
        ArgumentNullException.ThrowIfNull(services);

        GetOrAddRegistry(services).Add(priority, s => s.Decorate<TService, TDecorator>());

        return services;
    }

    /// <summary>
    /// Registers a decorator of type <paramref name="decoratorType"/> for service
    /// <paramref name="serviceType"/> with the specified priority.
    /// Lower priority numbers result in the decorator being the outermost wrapper
    /// (executed first). Call <see cref="ApplyDecoratorPriorities"/> to apply.
    /// </summary>
    public static IServiceCollection DecorateWithPriority(
        this IServiceCollection services,
        Type serviceType,
        Type decoratorType,
        int priority)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(decoratorType);

        GetOrAddRegistry(services).Add(priority, s => s.Decorate(serviceType, decoratorType));

        return services;
    }

    /// <summary>
    /// Registers a factory-based decorator for service <typeparamref name="TService"/> with
    /// the specified priority. The factory receives the inner service and the
    /// <see cref="IServiceProvider"/>. Lower priority numbers result in the decorator being
    /// the outermost wrapper (executed first). Call <see cref="ApplyDecoratorPriorities"/> to apply.
    /// </summary>
    public static IServiceCollection DecorateWithPriority<TService>(
        this IServiceCollection services,
        Func<TService, IServiceProvider, TService> decorator,
        int priority)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(decorator);

        GetOrAddRegistry(services).Add(priority, s => s.Decorate<TService>(decorator));

        return services;
    }

    /// <summary>
    /// Registers a factory-based decorator for service <typeparamref name="TService"/> with
    /// the specified priority. The factory receives the inner service.
    /// Lower priority numbers result in the decorator being the outermost wrapper
    /// (executed first). Call <see cref="ApplyDecoratorPriorities"/> to apply.
    /// </summary>
    public static IServiceCollection DecorateWithPriority<TService>(
        this IServiceCollection services,
        Func<TService, TService> decorator,
        int priority)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(decorator);

        GetOrAddRegistry(services).Add(priority, s => s.Decorate<TService>(decorator));

        return services;
    }

    /// <summary>
    /// Registers a factory-based decorator for <paramref name="serviceType"/> with
    /// the specified priority. The factory receives the inner service and the
    /// <see cref="IServiceProvider"/>. Lower priority numbers result in the decorator being
    /// the outermost wrapper (executed first). Call <see cref="ApplyDecoratorPriorities"/> to apply.
    /// </summary>
    public static IServiceCollection DecorateWithPriority(
        this IServiceCollection services,
        Type serviceType,
        Func<object, IServiceProvider, object> decorator,
        int priority)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(decorator);

        GetOrAddRegistry(services).Add(priority, s => s.Decorate(serviceType, decorator));

        return services;
    }

    /// <summary>
    /// Applies all decorators registered via <c>DecorateWithPriority</c> in priority order.
    /// Decorators with lower priority numbers are applied as the outermost wrapper and execute first.
    /// This method must be called after all <c>DecorateWithPriority</c> registrations.
    /// </summary>
    public static IServiceCollection ApplyDecoratorPriorities(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DecoratorRegistry));

        if (descriptor?.ImplementationInstance is not DecoratorRegistry registry)
        {
            return services;
        }

        foreach (var apply in registry.OrderedDecorations)
        {
            apply(services);
        }

        services.Remove(descriptor);

        return services;
    }

    private static DecoratorRegistry GetOrAddRegistry(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DecoratorRegistry));

        if (descriptor?.ImplementationInstance is DecoratorRegistry registry)
        {
            return registry;
        }

        var newRegistry = new DecoratorRegistry();
        services.AddSingleton(newRegistry);
        return newRegistry;
    }
}
