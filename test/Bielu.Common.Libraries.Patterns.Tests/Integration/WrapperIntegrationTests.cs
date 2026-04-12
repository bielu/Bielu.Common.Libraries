// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bielu.Common.Libraries.Patterns.Extensions;
using Bielu.Common.Libraries.Patterns.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bielu.Common.Libraries.Patterns.Tests.Integration;

/// <summary>
/// Integration tests that build a real <see cref="ServiceProvider"/> and verify that wrappers
/// are resolved correctly from the container.
/// </summary>
public class WrapperIntegrationTests
{
    // ─── Fixtures ────────────────────────────────────────────────────────────

    public interface INotificationService
    {
        string Send(string message);
    }

    public sealed class EmailNotificationService : INotificationService
    {
        public string Send(string message) => $"Email: {message}";
    }

    public sealed class NotificationServiceWrapper(INotificationService inner)
        : Wrapper<INotificationService>(inner)
    {
        public string SendUrgent(string message) => Inner.Send($"[URGENT] {message}");
    }

    public sealed class AuditedNotificationServiceWrapper(INotificationService inner, List<string> auditLog)
        : Wrapper<INotificationService>(inner)
    {
        public string SendAndAudit(string message)
        {
            var result = Inner.Send(message);
            auditLog.Add($"Audited: {message}");
            return result;
        }
    }

    // ─── Tests ───────────────────────────────────────────────────────────────

    [Fact]
    public void TransientWrapper_ResolvesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddTransientWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        var wrapper = provider.GetRequiredService<IWrapper<INotificationService>>();

        wrapper.ShouldBeOfType<NotificationServiceWrapper>();
        wrapper.Inner.ShouldBeOfType<EmailNotificationService>();
    }

    [Fact]
    public void TransientWrapper_InnerServiceDelegatesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddTransientWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        var wrapper = (NotificationServiceWrapper)provider.GetRequiredService<IWrapper<INotificationService>>();

        var result = wrapper.SendUrgent("System down");

        result.ShouldBe("Email: [URGENT] System down");
    }

    [Fact]
    public void TransientWrapper_InnerServiceCanBeUsedDirectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddTransientWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        var wrapper = provider.GetRequiredService<IWrapper<INotificationService>>();

        var result = wrapper.Inner.Send("Hello");

        result.ShouldBe("Email: Hello");
    }

    [Fact]
    public void ScopedWrapper_ResolvesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddScopedWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var wrapper = scope.ServiceProvider.GetRequiredService<IWrapper<INotificationService>>();

        wrapper.ShouldBeOfType<NotificationServiceWrapper>();
        wrapper.Inner.ShouldBeOfType<EmailNotificationService>();
    }

    [Fact]
    public void SingletonWrapper_ResolvesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddSingletonWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        var wrapper1 = provider.GetRequiredService<IWrapper<INotificationService>>();
        var wrapper2 = provider.GetRequiredService<IWrapper<INotificationService>>();

        wrapper1.ShouldBeSameAs(wrapper2);
    }

    [Fact]
    public void TransientWrapper_CreatesNewInstanceEachTime()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddTransientWrapper<INotificationService, NotificationServiceWrapper>();

        var provider = services.BuildServiceProvider();
        var wrapper1 = provider.GetRequiredService<IWrapper<INotificationService>>();
        var wrapper2 = provider.GetRequiredService<IWrapper<INotificationService>>();

        wrapper1.ShouldNotBeSameAs(wrapper2);
    }

    [Fact]
    public void FactoryWrapper_ResolvesCorrectly()
    {
        var auditLog = new List<string>();
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();
        services.AddSingleton(auditLog);
        services.AddTransientWrapper<INotificationService>(sp =>
            new AuditedNotificationServiceWrapper(
                sp.GetRequiredService<INotificationService>(),
                sp.GetRequiredService<List<string>>()));

        var provider = services.BuildServiceProvider();
        var wrapper = (AuditedNotificationServiceWrapper)provider.GetRequiredService<IWrapper<INotificationService>>();

        var result = wrapper.SendAndAudit("Test message");

        result.ShouldBe("Email: Test message");
        auditLog.ShouldBe(["Audited: Test message"]);
    }

    [Fact]
    public void Wrapper_ThrowsWhenInnerIsNull()
    {
        Should.Throw<ArgumentNullException>(() => new NotificationServiceWrapper(null!));
    }

    [Fact]
    public void FluentChaining_WorksCorrectly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INotificationService, EmailNotificationService>();

        var result = services
            .AddTransientWrapper<INotificationService, NotificationServiceWrapper>();

        result.ShouldBeSameAs(services);
    }
}
