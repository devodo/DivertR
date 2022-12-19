---
layout: default
title: Dependency Injection
nav_order: 3
has_children: true
---

# Dependency Injection

DivertR is designed to be embedded easily and transparently into the dependency injection (DI) container to facilitate testing an integrated, wired-up system.
It does this by decorating existing DI service registrations with [Redirects](./Redirect.md) that replace the originals.
These Redirects create proxies that wrap the instances resolved from the originals as their default targets or *roots*.

By default Redirect proxies transparently forward calls to their roots and therefore, in this initial state, the behaviour of the DI system is unchanged.
Then specific parts of the system can be modified as required by dynamically updating and resetting proxies between tests without requiring restart.

## .NET ServiceCollection

Out the box DivertR has support for the .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection`. The examples that follow use this `ServiceCollection` and its registered dependencies:

```csharp
IServiceCollection services = new ServiceCollection();

services.AddTransient<IFoo, Foo>();
services.AddSingleton<IBarFactory, BarFactory>();
services.AddSingleton<IEtc, Etc>();
```