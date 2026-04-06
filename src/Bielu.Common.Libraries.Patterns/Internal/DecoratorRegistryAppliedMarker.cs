// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Bielu.Common.Libraries.Patterns.Internal;

/// <summary>
/// Sentinel marker registered in <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
/// after <see cref="Extensions.DecoratorServiceCollectionExtensions.ApplyDecoratorPriorities"/> has been called.
/// Used to detect and prevent subsequent <c>DecorateWithPriority</c> calls.
/// </summary>
internal sealed class DecoratorRegistryAppliedMarker
{
    public static DecoratorRegistryAppliedMarker Instance { get; } = new();

    private DecoratorRegistryAppliedMarker()
    {
    }
}
