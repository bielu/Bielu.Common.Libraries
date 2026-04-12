// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Bielu.Common.Libraries.Patterns.Wrapper;

/// <summary>
/// Represents a wrapper around a service of type <typeparamref name="TService"/>.
/// Unlike a decorator (which implements the same interface and intercepts calls),
/// a wrapper exposes both its own API and direct access to the inner service.
/// </summary>
/// <typeparam name="TService">The type of the wrapped service.</typeparam>
public interface IWrapper<out TService>
{
    /// <summary>
    /// Gets the wrapped inner service.
    /// </summary>
    TService Inner { get; }
}
