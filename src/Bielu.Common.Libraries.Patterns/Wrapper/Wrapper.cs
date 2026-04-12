// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Bielu.Common.Libraries.Patterns.Wrapper;

/// <summary>
/// Abstract base class that wraps a service of type <typeparamref name="TService"/>.
/// Subclasses can expose additional functionality while providing access to the inner service.
/// </summary>
/// <typeparam name="TService">The type of the wrapped service.</typeparam>
public abstract class Wrapper<TService>(TService inner) : IWrapper<TService>
{
    /// <inheritdoc />
    public TService Inner { get; } = inner ?? throw new ArgumentNullException(nameof(inner));
}
