// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Extensions;
using Bielu.Common.Libraries.Patterns.Internal;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bielu.Common.Libraries.Patterns.Tests.Unit;

public class DecoratorServiceCollectionExtensionsTests
{
    // ─── Fixtures ────────────────────────────────────────────────────────────

    public interface IGreeter
    {
        string Greet(string name);
    }

    public sealed class RealGreeter : IGreeter
    {
        public string Greet(string name) => $"Hello, {name}";
    }

    public sealed class ShoutingGreeterDecorator(IGreeter inner) : IGreeter
    {
        public string Greet(string name) => inner.Greet(name).ToUpperInvariant();
    }

    public sealed class ExclamationGreeterDecorator(IGreeter inner) : IGreeter
    {
        public string Greet(string name) => inner.Greet(name) + "!";
    }

    // ─── Generic overload ────────────────────────────────────────────────────

    [Fact]
    public void DecorateWithPriority_Generic_AddsDecoratorRegistryToServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1);

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeTrue();
    }

    [Fact]
    public void DecorateWithPriority_Generic_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1));
    }

    // ─── Non-generic overload ────────────────────────────────────────────────

    [Fact]
    public void DecorateWithPriority_NonGeneric_AddsDecoratorRegistryToServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority(typeof(IGreeter), typeof(ShoutingGreeterDecorator), 1);

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeTrue();
    }

    [Fact]
    public void DecorateWithPriority_NonGeneric_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority(typeof(IGreeter), typeof(ShoutingGreeterDecorator), 1));
    }

    [Fact]
    public void DecorateWithPriority_NonGeneric_ThrowsWhenServiceTypeIsNull()
    {
        var services = new ServiceCollection();

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority(null!, typeof(ShoutingGreeterDecorator), 1));
    }

    [Fact]
    public void DecorateWithPriority_NonGeneric_ThrowsWhenDecoratorTypeIsNull()
    {
        var services = new ServiceCollection();

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority(typeof(IGreeter), (Type)null!, 1));
    }

    // ─── Factory overload with IServiceProvider ───────────────────────────

    [Fact]
    public void DecorateWithPriority_FactoryWithProvider_AddsDecoratorRegistryToServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority<IGreeter>(
            (inner, _) => new ShoutingGreeterDecorator(inner),
            priority: 1);

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeTrue();
    }

    [Fact]
    public void DecorateWithPriority_FactoryWithProvider_ThrowsWhenDecoratorIsNull()
    {
        var services = new ServiceCollection();
        Func<IGreeter, IServiceProvider, IGreeter> decorator = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority<IGreeter>(decorator, 1));
    }

    // ─── Factory overload without IServiceProvider ────────────────────────

    [Fact]
    public void DecorateWithPriority_FactoryWithoutProvider_AddsDecoratorRegistryToServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority<IGreeter>(
            inner => new ShoutingGreeterDecorator(inner),
            priority: 1);

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeTrue();
    }

    [Fact]
    public void DecorateWithPriority_FactoryWithoutProvider_ThrowsWhenDecoratorIsNull()
    {
        var services = new ServiceCollection();
        Func<IGreeter, IGreeter> decorator = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority<IGreeter>(decorator, 1));
    }

    // ─── Non-generic factory overload with IServiceProvider ──────────────

    [Fact]
    public void DecorateWithPriority_NonGenericFactoryWithProvider_AddsDecoratorRegistryToServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority(
            typeof(IGreeter),
            (inner, _) => new ShoutingGreeterDecorator((IGreeter)inner),
            priority: 1);

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeTrue();
    }

    [Fact]
    public void DecorateWithPriority_NonGenericFactoryWithProvider_ThrowsWhenServiceTypeIsNull()
    {
        var services = new ServiceCollection();

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority(null!, (inner, _) => inner, 1));
    }

    [Fact]
    public void DecorateWithPriority_NonGenericFactoryWithProvider_ThrowsWhenDecoratorIsNull()
    {
        var services = new ServiceCollection();
        Func<object, IServiceProvider, object> decorator = null!;

        Should.Throw<ArgumentNullException>(() =>
            services.DecorateWithPriority(typeof(IGreeter), decorator, 1));
    }

    // ─── ApplyDecoratorPriorities ─────────────────────────────────────────

    [Fact]
    public void ApplyDecoratorPriorities_ThrowsWhenServicesIsNull()
    {
        IServiceCollection services = null!;

        Should.Throw<ArgumentNullException>(() => services.ApplyDecoratorPriorities());
    }

    [Fact]
    public void ApplyDecoratorPriorities_RemovesRegistryFromServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();
        services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1);

        services.ApplyDecoratorPriorities();

        services.Any(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBeFalse();
    }

    [Fact]
    public void ApplyDecoratorPriorities_IsNoOpWhenNoDecoratorsPending()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();
        var countBefore = services.Count;

        services.ApplyDecoratorPriorities();

        services.Count.ShouldBe(countBefore);
    }

    [Fact]
    public void ApplyDecoratorPriorities_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();
        services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1);

        var result = services.ApplyDecoratorPriorities();

        result.ShouldBeSameAs(services);
    }

    // ─── Chaining / fluent API ────────────────────────────────────────────

    [Fact]
    public void DecorateWithPriority_Generic_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        var result = services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1);

        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void DecorateWithPriority_MultipleCallsShareSameRegistry()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGreeter, RealGreeter>();

        services.DecorateWithPriority<IGreeter, ShoutingGreeterDecorator>(1);
        services.DecorateWithPriority<IGreeter, ExclamationGreeterDecorator>(2);

        services.Count(d => d.ServiceType == typeof(DecoratorRegistry)).ShouldBe(1);
    }
}
