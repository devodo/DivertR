# Divertr

The .NET Dependency Injection Diverter

Divertr is a testing tool that lets you modify your dependency injection services at runtime.
Use it to replace your DI services with proxies that intercept calls and divert them to substitute instances such as test doubles.
The proxy behaviour can be reconfigured and reset on the fly while your app is running.

Divertr facilitates an integrated, top-down approach to testing by making it easy to hotswap in and out 
dependency injected parts of a working system with test code.

* [Quickstart](#quickstart)
    * [Create a Diversion proxy](#create-a-diversion-proxy)
    * [Configure a redirect](#configure-a-redirect)
    * [Redirect to a mock](#redirect-to-a-mock)
    * [Call the original instance](#call-the-original-instance)
    * [Divert multiple proxies](#divert-multiple-proxies)
    * [Chain redirects](#chain-redirects)
    * [Diverter (Diversion Set)](#diverter-diversion-set)
* [IServiceCollection Extensions](#iservicecollection-extensions)
    * [Register a Diverter](#register-a-diverter)
    * [Register a Diverter Set](#register-a-diverter-set) 

## Quickstart
### Create a Diversion proxy

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

Create a `Diversion` instance:

```csharp
var diversion = new Diversion<IFoo>();
```

Then use it to create a proxy:

```csharp
var foo = new Foo {Message = "hello foo"};
var fooProxy = diversion.Proxy(foo);
Console.WriteLine(fooProxy.Message); // "hello foo"
```
> By default Diversion proxies simply forward calls to the original instance. 

### Configure a redirect

Modify the proxy behaviour to redirect calls to a different instance:

```csharp
var altFoo = new Foo {Message = "hi Divertr"};
diversion.Redirect(altFoo);
Console.WriteLine(fooProxy.Message); // "hi Divertr"
```

Then reset the proxy to its default behaviour:

```csharp
diversion.Reset();
Console.WriteLine(fooProxy.Message); // "hello foo"
```

### Redirect to a mock

You can configure proxies to redirect to test doubles such as mocks:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns("mocked foo");

diversion.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "mocked foo"
```

### Call the original instance

The redirected instance can reference a proxy that forwards to the originally targeted instance from the `CallCtx.Root` property.
This allows you to intercept calls, run test code and then continue the original execution flow:

```csharp
var original = diversion.CallCtx.Root;
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{original.Message} bar");

diversion.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "hello foo bar"
```

> The `CallCtx.Root` property can be copied and reused. However its members can only be accessed
> within the context of call to its Diversion proxy. Attempting to invoke a member outside of this context will
> result in a `DiverterException` being thrown. 

### Divert multiple proxies

The configured redirects are applied to all proxies created from the same `Diversion` instance.
This allows you to divert calls from all instances of a given type to a single redirected instance.
This is useful, for example, to decorate the behaviours of transient DI services where multiple instances of a type can be created.

```csharp
var fooA = diversion.Proxy(new Foo {Message = "foo A"});
var fooB = diversion.Proxy(new Foo {Message = "foo B"});

var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"Hello {diversion.CallCtx.Original.Message}");

diversion.Redirect(mock.Object);

Console.WriteLine(fooA.Message); // "Hello foo A"
Console.WriteLine(fooB.Message); // "Hello foo B"
```

### Chain redirects

Multiple redirects can be appended together and referenced in the call allowing you to create a chain of responsibility pipeline.
The `CallCtx.Next` property proxies to the next redirect in the chain.
> If only a single redirect is registered the `Next` and `Root` properties proxy to the same original instance.

```csharp
var next = diversion.CallCtx.Next;
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{next.Message} bar");

diversion
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object);

Console.WriteLine(fooProxy.Message); // "hello foo bar bar bar"
```

### Diverter (Diversion Set)

A `Diversion<IFoo>` instance is tied to a single `IFoo` type but
typically when testing a system you would interact with multiple types.
The `Diverter` class lets you conveniently access and manage a set of different `Diversion<T>`
types from a single instance. This is particularly useful for setting up dependency 
injection registrations (see [Register Diverter](#register-diverter)). It is also
handy for resetting all `Diversions` from a single call to `ResetAll()`. 

```csharp
var diverter = new Diverter();

var diversion = diverter.Of<IFoo>();
diversion.Redirect(new Foo {Message = "Diverted"});
diverter.Of<IBar>().Redirect(new TestBar());

diverter.ResetAll();
```

## IServiceCollection Extensions
Extension methods are provided on the .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection` interface
that convert existing registrations into `Diversion` proxy factories.

### Register a Diversion

Starting with an `IServiceCollection` that has an `IFoo` registered:

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
```

The `Divert` extension method replaces the `IFoo` registration with a `Diversion` proxy factory that decorates over the original registration:

```csharp
var diversion = new Diversion<IFoo>();
services.Divert(diversion);
```

The subsequent `IServiceProvider` will now resolve `IFoo` `Diversion` proxy instances whose
behaviour can be modified by adding redirects to the `diversion` instance (in the same way as before):

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Message); // "Original"

diversion.Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"

diversion.Reset();
Console.WriteLine(foo.Message); // "Original"
```

### Register a Diverter

Similarly a `Diverter` extension method is provided that converts all existing `IServiceCollection` registrations to
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

> The default behaviour of `Diversion` proxies is to forward calls
to the original instances. Therefore replacing instances with these proxies
does not alter the behaviour of the system (when no redirects have been added).

The resulting `Diversion` proxies on all registered types are then configured via the passed in `diverter` instance:

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


