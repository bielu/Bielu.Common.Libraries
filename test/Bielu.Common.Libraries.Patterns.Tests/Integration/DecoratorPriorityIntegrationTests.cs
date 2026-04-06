// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bielu.Common.Libraries.Patterns.Tests.Integration;

/// <summary>
/// Integration tests that build a real <see cref="ServiceProvider"/> and verify that decorators
/// execute in the correct order after <see cref="DecoratorServiceCollectionExtensions.ApplyDecoratorPriorities"/>
/// is called.
/// </summary>
public class DecoratorPriorityIntegrationTests
{
    // ─── Fixtures ────────────────────────────────────────────────────────────

    public interface IOrderTracker
    {
        string Execute();
    }

    /// <summary>
    /// Records every step taken to a shared call log so tests can assert execution order.
    /// </summary>
    public sealed class RealOrderTracker(List<string> log) : IOrderTracker
    {
        public string Execute()
        {
            log.Add("Real");
            return "Real";
        }
    }

    public sealed class FirstDecorator(IOrderTracker inner, List<string> log) : IOrderTracker
    {
        public string Execute()
        {
            log.Add("First-before");
            var result = inner.Execute();
            log.Add("First-after");
            return $"First({result})";
        }
    }

    public sealed class SecondDecorator(IOrderTracker inner, List<string> log) : IOrderTracker
    {
        public string Execute()
        {
            log.Add("Second-before");
            var result = inner.Execute();
            log.Add("Second-after");
            return $"Second({result})";
        }
    }

    public sealed class ThirdDecorator(IOrderTracker inner, List<string> log) : IOrderTracker
    {
        public string Execute()
        {
            log.Add("Third-before");
            var result = inner.Execute();
            log.Add("Third-after");
            return $"Third({result})";
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static (IOrderTracker service, List<string> log) BuildWithPriorities(
        Action<IServiceCollection> configure)
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        configure(services);

        services.ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        return (provider.GetRequiredService<IOrderTracker>(), log);
    }

    // ─── Tests ───────────────────────────────────────────────────────────────

    [Fact]
    public void SingleDecorator_WrapsRealService()
    {
        var (service, log) = BuildWithPriorities(services =>
            services.DecorateWithPriority<IOrderTracker, FirstDecorator>(1));

        var result = service.Execute();

        result.ShouldBe("First(Real)");
        log.ShouldBe(["First-before", "Real", "First-after"]);
    }

    [Fact]
    public void TwoDecorators_RegisteredInOrder_FirstOuterSecondInner()
    {
        // Priority 1 = outermost, priority 2 = innermost
        var (service, log) = BuildWithPriorities(services =>
        {
            services.DecorateWithPriority<IOrderTracker, FirstDecorator>(1);
            services.DecorateWithPriority<IOrderTracker, SecondDecorator>(2);
        });

        var result = service.Execute();

        // Expected chain: First(Second(Real))
        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void TwoDecorators_RegisteredInReverseOrder_PriorityStillRespected()
    {
        // Registered in reverse (2 before 1) – priority should override registration order
        var (service, log) = BuildWithPriorities(services =>
        {
            services.DecorateWithPriority<IOrderTracker, SecondDecorator>(2);
            services.DecorateWithPriority<IOrderTracker, FirstDecorator>(1);
        });

        var result = service.Execute();

        // Expected chain: First(Second(Real)) regardless of registration order
        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void ThreeDecorators_CorrectExecutionOrder()
    {
        // Priority 1 = outermost, priority 3 = innermost
        var (service, log) = BuildWithPriorities(services =>
        {
            services.DecorateWithPriority<IOrderTracker, ThirdDecorator>(3);
            services.DecorateWithPriority<IOrderTracker, FirstDecorator>(1);
            services.DecorateWithPriority<IOrderTracker, SecondDecorator>(2);
        });

        var result = service.Execute();

        // Expected chain: First(Second(Third(Real)))
        result.ShouldBe("First(Second(Third(Real)))");
        log.ShouldBe(["First-before", "Second-before", "Third-before", "Real",
            "Third-after", "Second-after", "First-after"]);
    }

    [Fact]
    public void NonGenericTypeOverload_CorrectExecutionOrder()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        services.DecorateWithPriority(typeof(IOrderTracker), typeof(FirstDecorator), 1);
        services.DecorateWithPriority(typeof(IOrderTracker), typeof(SecondDecorator), 2);
        services.ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IOrderTracker>();

        var result = service.Execute();

        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void FactoryWithProviderOverload_CorrectExecutionOrder()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        services.DecorateWithPriority<IOrderTracker>(
            (inner, sp) => new FirstDecorator(inner, sp.GetRequiredService<List<string>>()),
            priority: 1);
        services.DecorateWithPriority<IOrderTracker>(
            (inner, sp) => new SecondDecorator(inner, sp.GetRequiredService<List<string>>()),
            priority: 2);

        services.ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IOrderTracker>();

        var result = service.Execute();

        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void FactoryWithoutProviderOverload_CorrectExecutionOrder()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        services.DecorateWithPriority<IOrderTracker>(
            inner => new FirstDecorator(inner, log),
            priority: 1);
        services.DecorateWithPriority<IOrderTracker>(
            inner => new SecondDecorator(inner, log),
            priority: 2);

        services.ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IOrderTracker>();

        var result = service.Execute();

        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void NonGenericFactoryOverload_CorrectExecutionOrder()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        services.DecorateWithPriority(
            typeof(IOrderTracker),
            (inner, sp) => new FirstDecorator((IOrderTracker)inner, sp.GetRequiredService<List<string>>()),
            priority: 1);
        services.DecorateWithPriority(
            typeof(IOrderTracker),
            (inner, sp) => new SecondDecorator((IOrderTracker)inner, sp.GetRequiredService<List<string>>()),
            priority: 2);

        services.ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IOrderTracker>();

        var result = service.Execute();

        result.ShouldBe("First(Second(Real))");
        log.ShouldBe(["First-before", "Second-before", "Real", "Second-after", "First-after"]);
    }

    [Fact]
    public void FluentChaining_WorksCorrectly()
    {
        var log = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton(log);
        services.AddSingleton<IOrderTracker, RealOrderTracker>();

        services
            .DecorateWithPriority<IOrderTracker, FirstDecorator>(1)
            .DecorateWithPriority<IOrderTracker, SecondDecorator>(2)
            .ApplyDecoratorPriorities();

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IOrderTracker>();

        var result = service.Execute();

        result.ShouldBe("First(Second(Real))");
    }
}
