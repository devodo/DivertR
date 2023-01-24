---
layout: default
title: Redirects
nav_order: 1
parent: Documentation
---

# Redirects

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

The `Redirect` is the main DivertR entity used to create and configure proxies.
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
{: .note }

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
{: .note }

# Vias

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

The `Via` intercepts any proxy calls matching the `To` expression 1. and diverts them to the `Via` delegate 2.

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

# Method Parameters

Via intercept match rules can be configured on method parameters using call argument values and these can also be passed to Via delegates.

## Parameter Matching

If the Via `To` expression specifies a method with parameters, these are matched to call arguments as follows:

```csharp
// Match calls to the Echo method with any argument value
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(() => "any");

// Match calls with arguments that satisfy a Match expression
fooRedirect
    .To(x => x.Echo(Is<string>.Match(a => a == "two")))
    .Via(() => "match");

// Match calls with arguments equal to a specified value
fooRedirect
    .To(x => x.Echo("three"))
    .Via(() => "equal");

Console.WriteLine(fooProxy.Echo("one")); // "any"
Console.WriteLine(fooProxy.Echo("two")); // "match"
Console.WriteLine(fooProxy.Echo("three")); // "equal"
```

## Call Arguments

Proxy call arguments can be passed to the Via delegate as follows:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => $"{call.Args[0]} redirected");
  
Console.WriteLine(fooProxy.Echo("me")); // "me redirected"
```
> The `Args` property is an `IReadOnlyList<object>` collection.
{: .note }

## Named Arguments

Strongly typed and named arguments can be specified by defining a `ValueTuple` generic type on the `Via` method as follows:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(call => $"{call.Args.input} redirected");

Console.WriteLine(fooProxy.Echo("me")); // "me redirected"
```

Call arguments are mapped in parameter order onto the `ValueTuple` items and it replaces the `Args` property from the previous example.

The special Diverter type `__` (double underscore) is used to specify a discard mapping that is ignored so that only named types of parameters that will be used need to be defined.

C# requires named ValueTuples to have at least two parameters. If the call only has a single parameter, as in the example above,
then the discard type `__` must be used to provide a second dummy parameter.

# Relay

A special feature of Redirects is their ability to control how calls are forwarded or *relayed* back to proxy root instances.

## Relay Root

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
{: .note }

## Relay Next

Any number of Vias can be added to a Redirect. When Vias are added they are pushed onto a stack (with the last added at the top).

![Via Stack]({{ site.baseurl }}/assets/images/Via_Stack.svg)

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
{: .note }

## Call Forwarding

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

# Additional Usages

## Async Methods

Async is fully supported by DivertR and Via delegates can be added to `Task` or `ValueTask` methods using the standard C# `async` syntax:

```csharp
fooRedirect
    .To(x => x.SaveAsync(Is<string>.Any, Is<CancellationToken>.Any))
    .Via(async call =>
    {
        var result = await call.CallNext();
        
        return result;
    });
```

## Property Getters and Setters

Vias for property getters, [demonstrated earlier](#vias), are added using the same `To` syntax as for standard methods. However, to indicate a Via is for a property setter, the `ToSet` method is used instead:

```csharp
fooRedirect
    .ToSet(x => x.Name)
    .Via<(string name, __)>(call =>
    {
        call.Next.Name = call.Args.name + " changed";
    });
```

By default the Via above will match any setter value input but the `ToSet` method accepts a second parameter as a value match expression using the usual [parameter matching](#parameter-matching) syntax:

```csharp
fooRedirect
    .ToSet(x => x.Name, () => Is<string>.Match(p => p.StartsWith("M")))
    .Via<(string name, __)>(call =>
    {
        call.Next.Name = name + " changed";
    });

fooProxy.Name = "Me";
Console.WriteLine(fooProxy.Name); // "Me changed"
```

## Void Methods

For methods that return `void`, the same `Redirect` fluent interface syntax is used, only the `Via` delegate provided is an `Action` rather than a `Func`:

```csharp
fooRedirect
    .To(x => x.SetAge(Is<int>.Any))
    .Via<(int age, __)>(call =>
    {
        call.Next.SetAge(call.Args.ags + 10);
    });
```

## Generic Methods

Generic method Vias are declared using the same fluent syntax ands are matched on the specified generic type arguments.

```csharp
fooRedirect
    .To(x => x.Echo<int>(Is<int>.Any))
    .Via(call => call.CallNext() * 2);
```

## Throwing Exceptions

Via delegates can throw exceptions using standard C# syntax and any exceptions thrown will bubble up to callers as usual:

```csharp
fooRedirect
    .To(x => x.Echo("exception"))
    .Via(() => throw new MyException())

fooProxy.Echo("exception"); // throws MyException
```

# Retarget

A Redirect can be configured to *retarget* its proxy calls to a substitute instance of its target type:

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo("MrFoo"));
Console.WriteLine(fooProxy.Name); // "MrFoo"

var fooTwo = new Foo("two");
Console.WriteLine(fooTwo.Name); // "two"

fooRedirect.Retarget(fooTwo);

Console.WriteLine(fooProxy.Name); // "two"
```

The retarget substitute can be any instance of the Redirect's target type including e.g. Mock objects:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Echo(It.IsAny<string>()))
    .Returns((string input) => $"{input} mock");

fooRedirect.Retarget(mock.Object);

Console.WriteLine(fooProxy.Echo("hello"));  // "hello mock"
```

When a `Retarget` is added it is also pushed onto the Redirect Via stack.
Retarget substitutes are also able to relay calls to the proxy root or next Via from the Redirect's `Relay` property:

```csharp
IFoo next = fooRedirect.Relay.Next;
IFoo root = fooRedirect.Relay.Root;
mock
    .Setup(x => x.Name)
    .Returns(() => $"{root.Name} {next.Name} mock");

Console.WriteLine(fooProxy.Name);  // "MrFoo two mock"
```

# Redirect Id

Every `Redirect<TTarget>` instance has a readonly `RedirectId` property that is a composite key made up of:

1. The `TTarget` type.
2. An optional `string` name (defaults to an empty `string` if not specified).

```csharp
var fooRedirect = new Redirect<IFoo>();

Assert.Equal(typeof(IFoo), fooRedirect.RedirectId.Type);
Assert.Equal(string.Empty, fooRedirect.RedirectId.Name);
Assert.Equal(fooRedirect.RedirectId, new RedirectId(typeof(IFoo)));
```

The optional name can be specified on creation:

```csharp
var fooRedirect2 = new Redirect<IFoo>("GroupX");
        
Assert.Equal(typeof(IFoo), fooRedirect2.RedirectId.Type);
Assert.Equal("GroupX", fooRedirect2.RedirectId.Name);
Assert.Equal(fooRedirect2.RedirectId, new RedirectId(typeof(IFoo), "GroupX"));
```

The `RedirectId` property is intended to be used as a key for indexing Redirects in collections such as the [RedirectSet](#redirect-set) discussed next.

# Redirect Set

The `RedirectSet` class manages a collection of `Redirect` instances that are unique by their [RedirectId](#redirect-id) key.
The main functions of `RedirectSet` are:

1. Create and internally store a set of `Redirect` instances.
2. Retrieve stored Redirects by `RedirectId`.
3. Perform `Redirect` actions such as `Reset` across all stored Redirects or groups of named subsets.

```csharp
// Instantiate a new RedirectSet
IRedirectSet redirectSet = new RedirectSet();
// Create and store a Redirect instance
IRedirect<IFoo> fooRedirect = redirectSet.GetOrCreate<IFoo>();
// The Redirect has already been created so the existing instance is returned
IRedirect<IFoo> fooRedirect2 = redirectSet.GetOrCreate<IFoo>();

Assert.Same(fooRedirect, fooRedirect2);

// Create and store sets of Redirects for different target types
var barRedirect = redirectSet.GetOrCreate<IBar>();
// Or multiple with the same type using names
var fooRedirect3 = redirectSet.GetOrCreate<IFoo>("GroupX");

Assert.NotNull(barRedirect);
Assert.NotEqual(fooRedirect, fooRedirect3);

// Perform a Redirect action across all Redirects in the set
redirectSet.ResetAll();
// Or across a subset by name
redirectSet.Reset("GroupX");
```

An important usage of `RedirectSet` is by the `Diverter` class when integrating with [dependency injection](../dependency_injection/) containers.

## Named Redirect

# Via Options

## Persistent Via

## Via Order

## Repeat

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
