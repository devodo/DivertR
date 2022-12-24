---
layout: default
title: Reset
nav_order: 3
parent: Redirects
---

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
