---
layout: default
title: Redirecting
nav_order: 1
parent: DivertR Proxies
---

# Redirects

Redirects are the main DivertR entities used to create and configure proxies.
`Redirect` instances are instantiated from the generic `Redirect<TTarget>` class:

```csharp
IRedirect<IFoo> fooRedirect = new Redirect<IFoo>();
```

# Creating Proxies

A `Redirect` creates proxy objects of its generic `TTarget` type.
E.g. an `IRedirect<IFoo>` like the one instantiated above creates `IFoo` proxies:

```csharp
var fooRedirect = new Redirect<IFoo>();

IFoo fooProxy = fooRedirect.Proxy(); // Create a proxy
IFoo fooTwo = fooRedirect.Proxy(); // Create another proxy
```

> A single Redirect can create any number of proxies.

## Proxy Root

When a proxy object is created it can be given a *root* instance of its target type.
The default behaviour of a proxy is to relay all calls to its root:

```csharp
IFoo fooRoot = new Foo("MrFoo");
Console.WriteLine(fooRoot.Name); // "MrFoo"

var fooRedirect = new Redirect<IFoo>();
IFoo fooProxy = fooRedirect.Proxy(fooRoot);
Console.WriteLine(fooProxy.Name); // "MrFoo"
```

When a proxy is in its initial state it is transparent, i.e. it behaves identically to its root instance. Therefore if a system has its instances replaced with transparent proxies its original behaviour is left unchanged.

## Dummy Root

If no root instance is provided then the proxy is created with a *dummy* root that returns default values:

```csharp
var fooRedirect = new Redirect<IFoo>();

var fooMock = fooRedirect.Proxy(); // Proxy created with dummy root
Console.WriteLine(fooMock.Name); // null
```

In general dummy roots return the .NET `default` of the call's return type, e.g. `null` for reference types and `0` for `int`.
There are some special cases such as `Task` types are returned as `null` valued completed Tasks.
> Proxies with dummy roots can be used as mock objects.

# Via

`Via` instances are added to a `Redirect` to control the way its proxies behave.
Proxy calls are diverted and passed to the Vias for handling.
A fluent interface is provided on the Redirect for building and adding Vias to itself:

```csharp
var foo = new Foo("MrFoo");
Console.WriteLine(foo.Name); // "MrFoo"

var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(foo);
Console.WriteLine(fooProxy.Name); // "MrFoo"

// Add a Via to the Redirect
fooRedirect
    .To(x => x.Name)         // 1. Match expression
    .Via(() => "Hello Via"); // 2. Via delegate

Console.WriteLine(fooProxy.Name); // "Hello Via"
```

The Via intercepts any proxy calls matching the `To` expression 1. and diverts them to the `Via` delegate 2.

Vias can be added to a Redirect at any time and apply immediately to all its existing proxies as well as any created afterwards.

# Reset

A Redirect can be *reset* which removes all its Vias, reverting its proxies to their original behaviour:

```csharp
fooRedirect.To(x => x.Name).Via("diverted");
Console.WriteLine(fooProxy.Name);  // "diverted"

fooRedirect.Reset();
  
Console.WriteLine(fooProxy.Name);  // "MrFoo"
```

Reset can be called at any time and is applied immediately to all of the Redirect's proxies.
By adding Vias and resetting, proxy behaviour can be modified at runtime allowing a running process to be altered between tests, e.g. to avoid restart and initialisation overhead.

After a Redirect is reset its proxies are in their default, transparent state of forwarding all calls to their root instances.
This enables a pattern of testing where proxy behaviour is modified with Vias and then the system is reset to its original state between tests.