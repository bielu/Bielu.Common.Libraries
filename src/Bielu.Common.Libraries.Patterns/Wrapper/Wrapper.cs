// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Bielu.Common.Libraries.Patterns.Wrapper;

/// <summary>
/// Abstract base class that wraps a service of type <typeparamref name="TService"/>.
/// Unlike a decorator (which implements the same service interface and intercepts calls),
/// a wrapper is an open implementation that exposes both its own API and direct access
/// to the inner service. Wrappers are registered in DI as their concrete type.
/// </summary>
/// <typeparam name="TService">The type of the wrapped service.</typeparam>
public abstract class Wrapper<TService>(TService inner)
{
    /// <summary>
    /// Gets the wrapped inner service.
    /// </summary>
    public TService Inner { get; } = inner ?? throw new ArgumentNullException(nameof(inner));
}
