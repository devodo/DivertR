---
layout: default
title: Dependency Injection
nav_order: 2
parent: Documentation
---

# Dependency Injection

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

DivertR is designed to be embedded easily and transparently into the dependency injection (DI) container to facilitate testing an integrated, wired-up system.
It does this by decorating existing DI service registrations with [Redirects](../redirects/) that replace the originals.
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

# Redirect Registration

First create an `IDiverter` instance by instantiating a `DiverterBuilder` class and *registering* one or more DI service types of interest that you would like to be able to *redirect*:

```csharp
var diverter = new DiverterBuilder()
    .Register<IFoo>()
    .Register<IBarFactory>()
    .Create();
```

Then call `Divert`, a provided `IServiceCollection` extension method, to install the registered types as `Redirect` decorators:

```csharp
services.Divert(diverter);
```

The `IServiceCollection` can now be used as usual to build the service provider and resolve dependency instances:

```csharp
IServiceProvider provider = services.BuildServiceProvider();

var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Name); // "Foo"

// The behaviour of the resolved foo is the same as its root e.g.:
IFoo demo = new Foo();
Console.WriteLine(demo.Name); // "Foo";
```

# Redirect Configuration

The resolved `IFoo` instance above is a *redirect* proxy generated by the underlying `IRedirect<IFoo>` decorator that uses the original DI registration to resolve the proxy root.
In its initial state the `IFoo` proxy forwards all calls directly to its root. However, this behaviour can be modified by obtaining the underlying `Redirect`
from the `Diverter` instance and adding a *Via*:

```csharp
// Obtain the underlying Redirect from the diverter instance
IRedirect<IFoo> fooRedirect = diverter.Redirect<IFoo>(); 

fooRedirect
    .To(x => x.Name)
    .Via(call => $"{call.CallNext()} diverted");

var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Name); // "Foo diverted"
```

Any Vias added to the `Redirect` are applied to all its existing proxies and any resolved afterwards:

```csharp
var foo2 = provider.GetService<IFoo>();
foo2.Name = "Foo2";
Console.WriteLine(foo2.Name); // "Foo2 diverted"
```

# Reset

All `Redirects` registered in the `Diverter` instance can be *reset* with a single call:

```csharp
diverter.ResetAll();
  
Console.WriteLine(foo.Name);  // "Foo"
Console.WriteLine(foo2.Name);  // "Foo2"
```

After reset all resolved redirect proxies will be returned to their initial state of forwarding calls to their roots.

# Persistent Builder Redirect Configurations

Registered redirects can also be configured from the `DiverterBuilder` using interface that is similar to the `IDiverter` fluent interface:

```csharp
var diverter = new DiverterBuilder()
    .Register<IFoo>()
    // Persistently configure the registered redirect
    .Redirect<IFoo>().To(x => x.Name).Via(call => $"{call.CallNext()} diverted")
    .Create();

// Install diverter in the ServiceCollection and resolve an IFoo
IFoo foo = new ServiceCollection()
    .AddTransient<IFoo, Foo>()
    .Divert(diverter)
    .BuildServiceProvider()
    .GetService<IFoo>();

// Builder redirect configurations are 'persistent' and do not get reset
diverter.ResetAll();
  
Console.WriteLine(foo.Name);  // "Foo diverted"
```

An important difference between `IDiverter` and `DiverterBuilder` configurations is that the latter are 'persistent' meaning they do not get removed on reset.
This offers a convenient way to add permanent configurations that do not get reset between tests.

# ViaRedirect to Redirect Inner Services

In some scenarios there are inner services that are not directly resolved by the dependency injection container such as those created by factories.

It is possible to redirect proxy these inner services by chaining them to DI registered redirects using the `ViaRedirect` configuration.
For example, if we have an `IBarFactory` factory service, resolved by the DI, that has factory methods that create `IBar` instances.
The redirect can be extended to `IBar` instances as follows:

```csharp
var diverter = new DiverterBuilder()
    .Register<IBarFactory>() // Register the redirect on the IBarFactory service
    .Redirect<IBarFactory>().ViaRedirect<IBar>()) // Extend to redirect any IBar instances returned by IBarFactory
    .Create();
```

> Configuring the `ViaRedirect` on the DiverterBuilder makes it persistent. 
{: .note }

Diverter is installed into the existing `IServiceCollection` as usual:

```csharp
// Existing service collection with its registrations
IServiceCollection services = new ServiceCollection();
services.AddSingleton<IBarFactory, BarFactory>();

// Install diverter with redirect registrations and nested registrations
services.Divert(diverter);

// Create the service provider
IServiceProvider provider = services.BuildServiceProvider();
```

Both the registered redirect types and extended redirect types are now resolved as `Redirect` proxies:

```csharp
var barFactory = provider.GetService<IBarFactory>(); // barFactory is an IRedirect<IBarFactory> proxy
IBar bar = barFactory.Create("MrBar"); // and the Create call returns IRedirect<IBar> proxies
Console.WriteLine(bar.Name); // "MrBar"

// Add a Via to alter behaviour
diverter
    .Redirect<IBar>
    .To(x => x.Name)
    .Via(call => call.Root.Name + " diverted");

Console.WriteLine(bar.Name); // "MrBar diverted"

// ResetAll also resets nested registrations
diverter.ResetAll();
Console.WriteLine(bar.Name); // "MrBar"
```

# Proxy Lifetime

To maintain the original system behaviour when DivertR replaces existing DI registrations with Redirect decorators the lifetime is preserved.

When instances are resolved from a Redirect decorated registration a new proxy is created each time but all from the same Redirect instance.
Therefore if the Redirect configuration is changed, e.g. by adding a Via, it is applied across all of the Redirect's proxy instances. 
This is important for managing proxies with lifetimes like transient where multiple instances could be resolved.

# Dispose

DivertR takes care to ensure DI resolved `Redirect` proxies and their root instances are correctly disposed. 

If a root instance implements the `IDisposable` interface then the DI container manages its disposal, as usual, according to its registration lifetime.

If the `Redirect` target type inherits `IDisposable` then **only** the proxy instances are disposed by the DI container and not the root.
In this case the responsibility is left to the proxy for forwarding the dispose call to its root (and it does this by default).

DivertR also supports and applies the same dispose behaviour to `IAsyncDisposable` types.