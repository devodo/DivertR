---
layout: default
title: Redirect Registration
nav_order: 1
parent: Dependency Injection
---

# Redirect Registration

First instantiate an instance of the `Diverter` class and *register* one or more DI service types of interest that you would like be wrapped as Redirect proxies:

```csharp
var diverter = new Diverter()
    .Register<IFoo>()
    .Register<IBarFactory>();
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
