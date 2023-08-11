---
layout: default
title: Spy
nav_order: 4
parent: Documentation
---

# Spy

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

The `Spy` class extends the core [Redirect](/redirect) to provide a convenient, familiar interface for standard mocking usage.

The main difference between `Spy` and `Redirect` is the spy interface has two additional read-only properties, `Mock` and `Calls`.
For a `Spy<TTarget>` instance, the `Mock` property is a proxy object of type `TTarget` and any calls to this object are recorded in the `Calls` properties.

Instantiate and use a Spy instance like this:

```csharp
// Instantiate an IFoo spy
ISpy<IFoo> fooSpy = new Spy<IFoo>();
// The Mock property is the spy's proxy object
IFoo fooMock = fooSpy.Mock;
// Out the box spy proxies return dummy values (C# defaults)
Assert.Null(fooMock.Name);
// For async methods a Task is returned wrapping a dummy result
Assert.Null(await fooMock.EchoAsync("test"));

// Proxy behaviour can be configured using the usual redirect fluent syntax
fooSpy.To(x => x.Name).Via(() => "Hello spy");
// Now matching calls are redirected to the Via delegate
Assert.Equal("Hello spy", fooMock.Name);

// Proxy calls are recorded to the Calls property
Assert.Equal(3, fooSpy.Calls.Count);
// Recorded calls can be filtered and verified
Assert.Equal(1, fooSpy.Calls.To(x => x.EchoAsync(Is<string>.Any)).Count);
```

## Proxy Root

When a `Spy` is created it can be given a *root* instance of its target type.
The default behaviour of the spy proxy is to relay all calls to its root:

```csharp
IFoo fooRoot = new Foo("MrFoo");
Assert.Equal("MrFoo", fooRoot.Name);

// Specify the proxy root at creation
var fooSpy = new Spy<IFoo>(fooRoot);
// By default proxy calls are relayed to the root
Assert.Equal("MrFoo", fooSpy.Mock.Name);
```

## Retarget

The proxy root can also be set or changed after creation by retargeting:

```csharp
IFoo fooRoot = new Foo("MrFoo");
Assert.Equal("MrFoo", fooRoot.Name);

// Create spy without proxy root
var fooSpy = new Spy<IFoo>();
Assert.Null(fooSpy.Mock.Name);

// Retarget to a new proxy root
fooSpy.Retarget(fooRoot);

// Proxy calls are now relayed to the set target
Assert.Equal("MrFoo", fooSpy.Mock.Name);
```

## Reset

Spies can be reset at any time. This clears recorded calls and removes all configured Vias.

```csharp
var fooSpy = new Spy<IFoo>();

fooSpy.To(x => x.Name).Via(() => "redirected");
Assert.Equal("redirected", fooSpy.Mock.Name);
Assert.Equal(1, fooSpy.Calls.Count);

// Reset spy
fooSpy.Reset();

// Call counts are reset
Assert.Equal(0, fooSpy.Calls.Count);
// And configured Vias removed
Assert.Null(fooSpy.Mock.Name);
```


## Static Spy Syntax

The `Spy.On` static method is provided as an alternative shorthand way to create spy proxies directly.
The `Spy.Of` static method can then be used to access the underlying `Spy` from the proxy. 

```csharp
var fooRoot = new Foo("MrFoo");
        
// Create a spy proxy with optional root using the Spy.On static method
IFoo fooProxy = Spy.On<IFoo>(fooRoot);

// The proxy instance relays calls to the root
Assert.Equal("MrFoo", fooProxy.Name);

// Use the Spy.Of static method to access and update spy configuration
Spy.Of(fooProxy)
    .To(x => x.Name)
    .Via(call => call.CallNext() + " spied");

Assert.Equal("MrFoo spied", fooProxy.Name);
Assert.Equal(2, Spy.Of(fooProxy).Calls.Count);
```


