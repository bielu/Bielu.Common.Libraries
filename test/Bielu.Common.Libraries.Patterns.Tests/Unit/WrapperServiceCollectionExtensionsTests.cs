// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Extensions;
using Bielu.Common.Libraries.Patterns.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bielu.Common.Libraries.Patterns.Tests.Unit;

public class WrapperServiceCollectionExtensionsTests
{
    // ─── Fixtures ────────────────────────────────────────────────────────────

    public interface IMyService
    {
        string DoWork();
    }

    public sealed class RealService : IMyService
    {
        public string DoWork() => "done";
    }

    public sealed class MyServiceWrapper(IMyService inner) : Wrapper<IMyService>(inner)
    {
        public string DoWorkUpperCase() => Inner.DoWork().ToUpperInvariant();
    }

    // ─── AddTransientWrapper<TService, TWrapper> ─────────────────────────

    [Fact]
    public void AddTransientWrapper_Generic_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddTransientWrapper<IMyService, MyServiceWrapper>();

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddTransientWrapper_Generic_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        var result = services.AddTransientWrapper<IMyService, MyServiceWrapper>();

        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddTransientWrapper_Generic_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddTransientWrapper<IMyService, MyServiceWrapper>());
    }

    // ─── AddScopedWrapper<TService, TWrapper> ────────────────────────────

    [Fact]
    public void AddScopedWrapper_Generic_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddScopedWrapper<IMyService, MyServiceWrapper>();

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddScopedWrapper_Generic_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        var result = services.AddScopedWrapper<IMyService, MyServiceWrapper>();

        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddScopedWrapper_Generic_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddScopedWrapper<IMyService, MyServiceWrapper>());
    }

    // ─── AddSingletonWrapper<TService, TWrapper> ─────────────────────────

    [Fact]
    public void AddSingletonWrapper_Generic_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddSingletonWrapper<IMyService, MyServiceWrapper>();

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddSingletonWrapper_Generic_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        var result = services.AddSingletonWrapper<IMyService, MyServiceWrapper>();

        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddSingletonWrapper_Generic_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddSingletonWrapper<IMyService, MyServiceWrapper>());
    }

    // ─── AddTransientWrapper<TService> (factory) ─────────────────────────

    [Fact]
    public void AddTransientWrapper_Factory_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddTransientWrapper<IMyService>(sp =>
            new MyServiceWrapper(sp.GetRequiredService<IMyService>()));

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddTransientWrapper_Factory_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        var result = services.AddTransientWrapper<IMyService>(sp =>
            new MyServiceWrapper(sp.GetRequiredService<IMyService>()));

        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddTransientWrapper_Factory_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddTransientWrapper<IMyService>(sp =>
                new MyServiceWrapper(sp.GetRequiredService<IMyService>())));
    }

    [Fact]
    public void AddTransientWrapper_Factory_ThrowsWhenFactoryIsNull()
    {
        var services = new ServiceCollection();
        Func<IServiceProvider, IWrapper<IMyService>> factory = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddTransientWrapper<IMyService>(factory));
    }

    // ─── AddScopedWrapper<TService> (factory) ────────────────────────────

    [Fact]
    public void AddScopedWrapper_Factory_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddScopedWrapper<IMyService>(sp =>
            new MyServiceWrapper(sp.GetRequiredService<IMyService>()));

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddScopedWrapper_Factory_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddScopedWrapper<IMyService>(sp =>
                new MyServiceWrapper(sp.GetRequiredService<IMyService>())));
    }

    [Fact]
    public void AddScopedWrapper_Factory_ThrowsWhenFactoryIsNull()
    {
        var services = new ServiceCollection();
        Func<IServiceProvider, IWrapper<IMyService>> factory = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddScopedWrapper<IMyService>(factory));
    }

    // ─── AddSingletonWrapper<TService> (factory) ─────────────────────────

    [Fact]
    public void AddSingletonWrapper_Factory_RegistersWrapperInServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMyService, RealService>();

        services.AddSingletonWrapper<IMyService>(sp =>
            new MyServiceWrapper(sp.GetRequiredService<IMyService>()));

        services.Any(d => d.ServiceType == typeof(IWrapper<IMyService>)).ShouldBeTrue();
    }

    [Fact]
    public void AddSingletonWrapper_Factory_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddSingletonWrapper<IMyService>(sp =>
                new MyServiceWrapper(sp.GetRequiredService<IMyService>())));
    }

    [Fact]
    public void AddSingletonWrapper_Factory_ThrowsWhenFactoryIsNull()
    {
        var services = new ServiceCollection();
        Func<IServiceProvider, IWrapper<IMyService>> factory = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.AddSingletonWrapper<IMyService>(factory));
    }
}
