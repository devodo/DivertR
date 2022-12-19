---
layout: default
title: Strict Mode
nav_order: 7
parent: Redirect
---

# Strict mode

Enable *strict mode* on a Redirect to ensure only methods with matching Vias are allowed to be called:

```csharp
fooRedirect.Strict(); // enables strict mode

fooRedirect
    .To(x => x.Echo("ok"))
    .Via(call => call.Args[0]);

fooProxy.Echo("me"); // throws StrictNotSatisfiedException
fooProxy.Echo("ok"); // "ok" 
```

When strict mode is enabled a `StrictNotSatisfiedException` is thrown if a call is made to a proxy and does not match any Vias.

Strict mode is disabled when a Redirect is created or [reset](#reset).
