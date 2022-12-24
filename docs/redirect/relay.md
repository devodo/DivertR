---
layout: default
title: Relay Calls
nav_order: 5
parent: Redirects
---

# Relay Calls

A special feature of Redirects is their ability to control how calls are forwarded or *relayed* back to proxy root instances.

## Relay root

The Via delegate can *relay* calls back to the proxy root by calling the `Relay.Root` property:

```csharp
fooRedirect
    .To(x => x.Name)
    .Via(call =>
    {
        IFoo root = call.Relay.Root;
        return $"{root.Name} relayed";
    });

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo relayed"
```

> The `Relay.Root` property is a proxy that relays calls to the root instance.

## Relay next

Any number of Vias can be added to a Redirect. When Vias are added they are pushed onto a stack (with the last added at the top).

![Via Stack]({{ site.url }}/assets/images/Via_Stack.svg)

Proxy calls are traversed through the stack from top to bottom. If a call matches the `To` constraint it is passed to the Via delegate for handling.
If no Vias match, the call falls through the stack to the root instance.

Via delegates can relay the call directly to the root as in the previous example
but they can also continue the call down the Via stack by calling the `Relay.Next` property as follows:

```csharp
fooRedirect
    .To(x => x.Name)
    .Via(call => $"{call.Relay.Next.Name} 1")
    .Via(call => $"{call.Relay.Next.Name} 2") // Via calls can be chained
    .Via(call => $"{call.Next.Name} 3"); // The Root and Next properties can be accessed directly from the call argument

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1 2 3"
```

> The `Relay.Next` property is a proxy that relays calls to the next Via that matches.
> If no Vias match it will relay to the root.
> The Root and Next properties can also be accessed directly from the call argument for convenience.

## Call forwarding

A Via can call `CallRoot()` to forward the call to the target method of the root instance:

```csharp
fooRedirect
    .To(x => x.Name)
    .Via(call => call.CallRoot() + " 1");

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1"
```

Or the call can be forwarded down the Via stack using `CallNext()`:

```csharp
fooRedirect
    .To(x => x.Name)
    .Via(call => call.CallNext() + " 1")
    .Via(call => call.CallNext() + " 2");

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1 2"
```

If the target method has parameters then `CallRoot()` and `CallNext()` forward the arguments from the original call:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => call.CallNext() + " and you");

Console.WriteLine(fooProxy.Echo("me")); // "me and you"
```

Custom arguments can be forwarded by passing an `object[]` to `CallRoot()` or `CallNext()`:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => call.CallNext(new[] { "you" }));

Console.WriteLine(fooProxy.Echo("me")); // "you"
```
