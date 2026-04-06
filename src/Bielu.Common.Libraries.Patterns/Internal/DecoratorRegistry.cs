// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace Bielu.Common.Libraries.Patterns.Internal;

internal sealed class DecoratorRegistry
{
    private readonly List<(int Priority, Action<IServiceCollection> Apply)> _decorations = [];

    public void Add(int priority, Action<IServiceCollection> apply)
    {
        _decorations.Add((priority, apply));
    }

    /// <summary>
    /// Returns decoration actions sorted so that higher-priority-number decorators are applied first.
    /// Because Scrutor's last Decorate() call becomes the outermost wrapper, applying in descending
    /// priority order ensures that the lowest priority number ends up as the outermost decorator
    /// (i.e. it executes first when the service is called).
    /// </summary>
    public IEnumerable<Action<IServiceCollection>> OrderedDecorations =>
        _decorations
            .OrderByDescending(d => d.Priority)
            .Select(d => d.Apply);
}
