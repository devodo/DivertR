---
layout: default
title: Proxy
nav_order: 1
parent: Redirect
---

# Proxy

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
