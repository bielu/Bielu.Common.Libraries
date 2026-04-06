# Bielu.Common.Libraries

[![Build & Publish](https://github.com/bielu/Bielu.Common.Libraries/actions/workflows/buildAndPublishPackage.yml/badge.svg)](https://github.com/bielu/Bielu.Common.Libraries/actions/workflows/buildAndPublishPackage.yml)
[![NuGet](https://img.shields.io/nuget/v/Bielu.Common.Libraries.Patterns.svg)](https://www.nuget.org/packages/Bielu.Common.Libraries.Patterns)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A collection of reusable .NET libraries for the Bielu ecosystem, providing common patterns and utilities for dependency injection.

## Packages

| Package | Description |
|---------|-------------|
| [Bielu.Common.Libraries.Patterns](https://www.nuget.org/packages/Bielu.Common.Libraries.Patterns) | Priority-based decorator registration extensions for [Scrutor](https://github.com/khellang/Scrutor) |

## Bielu.Common.Libraries.Patterns

### The Problem

When using the [Scrutor](https://github.com/khellang/Scrutor) library to register decorators in the .NET dependency injection container, the order in which decorators wrap a service is determined solely by their registration order. This becomes difficult to manage when decorators are registered across multiple modules or packages — you have no explicit control over the execution order.

### The Solution

`Bielu.Common.Libraries.Patterns` introduces **priority-based decorator registration**. Each decorator is registered with an explicit priority number, and the library ensures they are applied in the correct order regardless of registration sequence.

- **Lower priority numbers** → outermost decorator (executes **first**)
- **Higher priority numbers** → innermost decorator (executes **last**, closest to the real service)

### Installation

```shell
dotnet add package Bielu.Common.Libraries.Patterns
```

### Quick Start

```csharp
using Bielu.Common.Libraries.Patterns.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register your service
services.AddSingleton<IMyService, MyService>();

// Register decorators with explicit priorities (order of registration doesn't matter)
services.DecorateWithPriority<IMyService, LoggingDecorator>(1);    // outermost — executes first
services.DecorateWithPriority<IMyService, CachingDecorator>(2);    // middle
services.DecorateWithPriority<IMyService, ValidationDecorator>(3); // innermost — executes last

// Apply all decorators in priority order
services.ApplyDecoratorPriorities();

var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IMyService>();
// Execution chain: Logging → Caching → Validation → MyService
```

### API Reference

All methods are extension methods on `IServiceCollection` and support fluent chaining.

#### `DecorateWithPriority<TService, TDecorator>(int priority)`

Registers a decorator type with a given priority.

```csharp
services.DecorateWithPriority<IMyService, LoggingDecorator>(1);
```

#### `DecorateWithPriority(Type serviceType, Type decoratorType, int priority)`

Non-generic overload for scenarios where types are determined at runtime.

```csharp
services.DecorateWithPriority(typeof(IMyService), typeof(LoggingDecorator), 1);
```

#### `DecorateWithPriority<TService>(Func<TService, IServiceProvider, TService> decorator, int priority)`

Factory-based decorator with access to the `IServiceProvider`.

```csharp
services.DecorateWithPriority<IMyService>(
    (inner, sp) => new LoggingDecorator(inner, sp.GetRequiredService<ILogger>()),
    priority: 1);
```

#### `DecorateWithPriority<TService>(Func<TService, TService> decorator, int priority)`

Factory-based decorator with access to only the inner service.

```csharp
services.DecorateWithPriority<IMyService>(
    inner => new LoggingDecorator(inner),
    priority: 1);
```

#### `DecorateWithPriority(Type serviceType, Func<object, IServiceProvider, object> decorator, int priority)`

Non-generic factory-based decorator with access to the `IServiceProvider`.

```csharp
services.DecorateWithPriority(
    typeof(IMyService),
    (inner, sp) => new LoggingDecorator((IMyService)inner),
    priority: 1);
```

#### `ApplyDecoratorPriorities()`

Applies all registered decorators in priority order. **Must be called after all `DecorateWithPriority` registrations.** Any subsequent calls to `DecorateWithPriority` will throw an `InvalidOperationException`.

```csharp
services.ApplyDecoratorPriorities();
```

### Fluent Chaining

All methods return `IServiceCollection`, so you can chain calls:

```csharp
services
    .DecorateWithPriority<IMyService, LoggingDecorator>(1)
    .DecorateWithPriority<IMyService, CachingDecorator>(2)
    .DecorateWithPriority<IMyService, ValidationDecorator>(3)
    .ApplyDecoratorPriorities();
```

## Requirements

- .NET 10.0 or later

## Building

```shell
dotnet build src/Bielu.Common.Libraries.slnx
```

## Testing

```shell
dotnet test src/Bielu.Common.Libraries.slnx
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## Author

Arkadiusz Biel ([@bielu](https://github.com/bielu))