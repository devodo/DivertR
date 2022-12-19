---
layout: default
title: Via Redirect
nav_order: 3
parent: Dependency Injection
---

# Via Redirect

Sometimes a test needs to manipulate instances that are not directly created by the DI container.
E.g. if we assume the `IBarFactory` service registration given above is a factory that creates `IBar` instances.
These instances can be wrapped and managed as Redirect proxies by calling `ViaRedirect` as follows:

```csharp

// Wrap created IBar instances as Redirect proxies and get a reference their Redirect
IRedirect<IBar> barRedirect = diverter
    .Redirect<IBarFactory>()
    .To(x => x.Create(Is<string>.Any))
    .ViaRedirect();

var barFactory = provider.GetService<IBarFactory>();
IBar bar = barFactory.Create("MrBar"); // The Create call now returns IBar proxies
Console.WriteLine(bar.Name); // "MrBar"

// Add a Via to alter behaviour
barRedirect
   .To(x => x.Name)
   .Via(call => call.Root.Name + " diverted");

Console.WriteLine(bar.Name); // "MrBar diverted"

// ResetAll also resets ViaRedirects
diverter.ResetAll();
Console.WriteLine(bar.Name); // "MrBar"
```

`RedirectVia` intercepts the method return values and wraps them as proxies created from a Via.
It returns this Via that can then be used to control the behaviour of the proxy wrappers.
