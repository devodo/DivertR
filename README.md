# Divertr

The .NET Dependency Injection Diverter

Divertr is a testing tool that lets you modify your dependency injection services at runtime.
Use it to replace your DI services with proxies that intercept calls and divert them to substitute instances such as test doubles.
The proxy behaviour can be reconfigured and reset on the fly while your app is running.

This facilitates an integrated, top-down approach to testing by making it easy to hotswap in and out 
dependency injected parts of a working system with test code.

* [Quickstart](#quickstart)
    * [Create a Divertr proxy](#create-a-divertr-proxy)
    * [Configure a redirect](#configure-a-redirect)
    * [Redirect to a mock](#redirect-to-a-mock)
    * [Call the original instance](#call-the-original-instance)
    * [Divert multiple proxies](#divert-multiple-proxies)
    * [Chain redirects](#chain-redirects)
    * [Diverter Sets](#diverter-sets)
* [IServiceCollection Extensions](#iservicecollection-extensions)
    * [Register a Diverter](#register-a-diverter)
    * [Register a Diverter Set](#register-a-diverter-set) 

## Quickstart
### Create a Divertr proxy

Given an `IFoo` interface and its implementation:

```csharp
public interface IFoo
{
    string Message { get; }
}

public class Foo : IFoo
{
    public string Message { get; set; }
}
```

Create a Divertr instance:

```csharp
var diverter = new Diverter<IFoo>();
```

Then use it to create a proxy:

```csharp
var foo = new Foo {Message = "hello foo"};
var fooProxy = diverter.Proxy(foo);
Console.WriteLine(fooProxy.Message); // "hello foo"
```
> By default Divertr proxies simply forward calls to the original instance. 

### Configure a redirect

Modify the proxy behaviour to redirect calls to a different instance:

```csharp
var altFoo = new Foo {Message = "hello Divertr"};
diverter.Redirect(altFoo);
Console.WriteLine(fooProxy.Message); // "hello Divertr"
```

Then reset the proxy to its default behaviour:

```csharp
diverter.Reset();
Console.WriteLine(fooProxy.Message); // "hello foo"
```

### Redirect to a mock

You can configure proxies to redirect to test doubles such as mocks:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns("bye bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "bye bar"
```

### Call the original instance

The redirected instance can reference a proxy that forwards to the originally targeted instance from the `CallCtx.Original` property.
This allows you, for example, to intercept calls, enrich and inspect, and then continue the original execution flow:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{diverter.CallCtx.Original.Message} bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "hello foo bar"
```

> The `CallCtx.Original` property can be copied and reused. However its members can only be called
> within the context of call to an associated Divertr proxy. Attempting to invoke a member outside of this context will
> result in an exception being thrown. 

### Divert multiple proxies

The configured redirects are applied to all proxies created from the same Divertr instance.
This allows you to divert calls from all instances of a given type to a single redirected instance.
This is useful, for example, to decorate the behaviours of transient DI services where multiple instances of a type can be created.

```csharp
var fooA = diverter.Proxy(new Foo {Message = "foo A"});
var fooB = diverter.Proxy(new Foo {Message = "foo B"});

var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"Hello {diverter.CallCtx.Original.Message}");

diverter.Redirect(mock.Object);

Console.WriteLine(fooA.Message); // "Hello foo A"
Console.WriteLine(fooB.Message); // "Hello foo B"
```

### Chain redirects

Multiple redirects can be appended together and referenced in the call allowing you to create a chain of responsibility pipeline.
The `CallCtx.Replaced` property proxies to the previous redirect in the chain.
> If only a single redirect is registered the `Replaced` and `Original` properties proxy to the same original instance.

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{diverter.CallCtx.Replaced.Message} bar");

diverter
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object);

Console.WriteLine(fooProxy.Message); // "hello foo bar bar bar"
```

### Diverter Sets

A diverter instance is typed to a single `IDiverter<T>` interface but
typically when testing a system you would interact with multiple types.
A `DiverterSet` lets you conveniently access and manage a set of diverters
from a single instance. This is particularly useful for setting up dependency 
injection registrations (see [Register a Diverter Set](#register-a-diverter-set)). It is also
handy for resetting all diverters from a single call to `ResetAll()`. 

```csharp
var diverterSet = new DiverterSet();

var diverter = diverterSet.Get<IFoo>();
diverter.Redirect(new Foo {Message = "Diverted"});
diverterSet.Get<IBar>().Redirect(new TestBar());

diverterSet.ResetAll();
```

## IServiceCollection Extensions
Extension methods are provided on the .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection` interface
that convert existing registrations into Divertr proxy factories.

### Register a Diverter

Starting with an `IServiceCollection` that has an `IFoo` registered:

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
```

The `Divert` extension method replaces the `IFoo` registration with a Divertr proxy factory that decorates the original:

```csharp
var diverter = new Diverter<IFoo>();
services.Divert(diverter);
```

The subsequent `IServiceProvider` will now resolve `IFoo` Divertr proxy instances whose
behaviour can be modified by adding redirects to the `diverter` instance (in the same way as before):

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Message); // "Original"

diverter.Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"

diverter.Reset();
Console.WriteLine(foo.Message); // "Original"
```

### Register a Diverter Set

Similarly a `DiverterSet` extension method is provided that converts all existing `IServiceCollection` registrations to
Divertr proxy factories at once.

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
services.AddSingleton<IBar, Bar>();
```

Passing a `DiverterSet` to the `Divert` extension method will convert both the `IFoo` and `IBar` registrations
into Divertr proxy factories:

```csharp
var diverterSet = new DiverterSet();
services.Divert(diverterSet);
```

> The default behaviour of Divertr proxies is to forward calls
to the original instances. Therefore replacing instances with Divertr proxies
does not alter the behaviour of the system (when no redirects have been added).

The resulting Divertr proxies on all registered types are then configured via the passed in `diverterSet` instance:

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();

diverterSet.Get<IFoo>().Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"
```

The redirects across all types can be reset by a single call to `ResetAll()`:

```csharp
diverterSet.ResetAll();
Console.WriteLine(foo.Message); // "Original"
```


