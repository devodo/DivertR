---
layout: default
title: Via
nav_order: 2
parent: Redirect
---

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
