# DivertR

The .NET Dependency Injection Diverter

DivertR is a tool that facilitates an integrated, top-down approach to testing by making it easy to hotswap test code in and out of a working system.

With DivertR you can modify your dependency injection services at runtime by replacing them with configurable proxies.
These can redirect calls to substitute, such as test doubles, and then optionally
relay back to the original instances. 
Update and reset proxies, on the fly, while the process is running.

![DivertR Via](./docs/assets/images/DivertR_Via.svg)

* [Quickstart](#quickstart)
    * [Create a Via proxy](#create-a-via-proxy)
    * [Redirect proxy calls](#redirect-proxy-calls)
    * [Redirect to a mock](#redirect-to-a-mock)
    * [Relay to the original](#relay-to-the-original)
    * [Divert multiple proxies](#divert-multiple-proxies)
    * [Chain redirects](#chain-redirects)
    * [The Diverter class](#the-diverter-class)
    * [Diverter shortcuts](#diverter-shortcuts)
    * [Async support](#async-support)
    * [Class support](#class-support)
* [IServiceCollection Extensions](#iservicecollection-extensions)
    * [Register a Via](#register-a-via)
    * [Register a Diverter](#register-a-diverter)
    * [Diverter registration builder](#diverter-registration-builder)

## Quickstart
### Create a Via proxy

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

Create a `Via` instance:

```csharp
var via = new Via<IFoo>();
```

Then use it to create a proxy:

```csharp
var foo = new Foo {Message = "original foo"};
var fooProxy = via.Proxy(foo);
Console.WriteLine(fooProxy.Message); // "original foo"
```
> By default Via proxies simply forward calls to their original instances. 

### Redirect proxy calls

Configure the `Via` to redirect its proxy calls to a different instance:

```csharp
var altFoo = new Foo {Message = "hi DivertR"};
via.Redirect(altFoo);
Console.WriteLine(fooProxy.Message); // "hi DivertR"
```

Then reset the `Via` and its proxy defaults back to the original instance:

```csharp
via.Reset();
Console.WriteLine(fooProxy.Message); // "original foo"
```

### Redirect to a mock

You can configure the `Via` to redirect its proxies to test doubles such as mocks:

```csharp
var mock = new Mock<IFoo>();
mock
  .Setup(x => x.Message)
  .Returns("mocked foo");

via.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "mocked foo"
```

### Relay to the original

The redirected instance can relay calls to the originally targeted instance from the `Relay.Original` property.
This allows you to intercept calls and then run test code before and after invoking the original instance:

```csharp
var original = via.Relay.Original;
var mock = new Mock<IFoo>();
mock
  .Setup(x => x.Message)
  .Returns(() => ${
    // run test code before
    // ...

    // call original instance
    var message = original.Message;

    // run test code after
    // ...

    return $"{message} bar";
  });

via.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "original foo bar"
```

> The `Relay.Original` property is a proxy that can be copied and reused. However its members can only be accessed
> within the context of intercepted calls on its `Via` proxies. Attempting to invoke a member outside of this context will
> result in a `DiverterException` being thrown. 

### Divert multiple proxies

The configured redirects are applied to all proxies created from the same `Via` instance.
This is useful, for example, to be able to intercept transient DI services where multiple instances of a type may get created.

```csharp
var original = via.Relay.Original;
var mock = new Mock<IFoo>();
mock
  .Setup(x => x.Message)
  .Returns(() => $"Hello {original.Message}");

via.Redirect(mock.Object);

var fooA = via.Proxy(new Foo {Message = "foo A"});
var fooB = via.Proxy(new Foo {Message = "foo B"});

Console.WriteLine(fooA.Message); // "Hello foo A"
Console.WriteLine(fooB.Message); // "Hello foo B"
```
> Note the same relay instance proxies to the correct original instance associated with the call. 
> Also note redirects can be configured before or after the proxies are created.

### Chain redirects

Multiple redirects can be appended together and relayed to within a call as a chain of responsibility pipeline.
The `Relay.Next` property proxies to the next redirect in the chain from the last redirect added up to and including the original instance.

```csharp
var next = via.Relay.Next;
var mock = new Mock<IFoo>();
mock
  .Setup(x => x.Message)
  .Returns(() => $"{next.Message} bar");

via
  .AddRedirect(mock.Object)
  .AddRedirect(mock.Object)
  .AddRedirect(mock.Object);

Console.WriteLine(fooProxy.Message); // "original foo bar bar bar"
```
> If only a single redirect is added the `Next` and `Original` properties proxy to the same original instance.

### The Diverter class

A `Via<T>` instance can only be used to proxy instances of type `T` but
typically when testing a system you interact with multiple types.
The `Diverter` class lets you conveniently access and manage a set of Vias instances of different
types from a single instance. This is particularly useful for setting up dependency 
injection registrations (see [Register a Diverter](#register-a-diverter)). It is also
handy for resetting all its `Via` instances from a single call to `ResetAll()`. 

```csharp
var diverter = new Diverter();

var via = diverter.Via<IFoo>();
via.Redirect(new Foo {Message = "Diverted"});
diverter.Via<IBar>().Redirect(new TestBar());

diverter.ResetAll();
```

### Diverter shortcuts

For convenience the `Diverter` class provides shortcut helpers for easier access
to most of the `Via` methods:

```csharp
var diverter = new Diverter();

var proxy = diverter.Proxy<IFoo>(new Foo {Message = "original"});
// Shortcut equivalent of:
// var proxy = diverter.Via<IFoo>().Proxy(new Foo {Message = "original"});

// Similarly other shortcuts for:
var relay = diverter.Relay<IFoo>().Original;
diverter.Redirect<IFoo>(new Foo {Message = "diverted"});
diverter.Reset<IFoo>();
// etc
```

### Async support

Internally DivertR uses `AsyncLocal` to track the ambient context of the current call.
Async calls are therefore fully supported on proxy and relay methods e.g.:

```csharp
public interface IBarRepository
{
    Task<Bar> GetBarAsync(Guid id);
}

var mock = new Mock<IBarRepository>();
mock
  .Setup(x => x.GetBarAsync(It.IsAny<Guid>()))
  .Returns(async (Guid id) => {
    // Run test code before
    var bar = await diverter.Relay<IBarRepository>().Original.GetBarAsync(id);
    // Run test code after
    return bar;
  });

diverter.Redirect<IBarRepository>(mock.Object);
var proxy = diverter.Proxy<IBarRepository>(barRepository);
var result = await proxy.GetBarAsync(barId);
```
> Note all DivertR methods are thread safe. However the thread safety of proxy and relay calls depends
> on the underlying thread safety of the redirect and original instances that you input.

### Class support

DivertR only supports diverting instances via their interfaces. If you instantiate a `new Via<T>()` and `T` is
not an interface type a runtime exception will be thrown.

Internally DivertR uses Castle Dynamic Proxy to generate proxies and intercept calls.
Therefore in the future DivertR may be extended to support virtual and abstract methods on classes
but for now only interfaces are supported.


## IServiceCollection Extensions
Extension methods are provided on the .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection` interface
that convert existing registrations into Via proxy factories.

### Register a Via

Starting with an `IServiceCollection` that has an `IFoo` registered:

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
```

The `Divert` extension method replaces the `IFoo` registration with a factory that creates Via proxies
wrapping the instances provided by the original registration:

```csharp
var via = new Via<IFoo>();
services.Divert(via);
```

The subsequent `IServiceProvider` will now resolve `IFoo` proxies of the `Via` instance that
can be configured and redirected as before:

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Message); // "Original"

via.Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"

via.Reset();
Console.WriteLine(foo.Message); // "Original"
```

### Register a Diverter

Similarly a `Diverter` extension method is provided that replaces all existing `IServiceCollection` registrations with
Via proxy factories at once.

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
services.AddSingleton<IBar, Bar>();
```

Passing a `Diverter` instance to the `Divert` extension method will convert both the `IFoo` and `IBar` registrations
into Via proxy factories:

```csharp
var diverter = new Diverter();
services.Divert(diverter);
```

> By default Via proxies forward calls to their original instances. Therefore injecting Via proxies
does not alter the behaviour of the system (when no redirects have been added).

The resulting Via proxies are then configured via the passed in `diverter` instance:

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();

diverter.Redirect<IFoo>(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"
```

All redirects configured on the diverter instance can be reset by a single call to `ResetAll()`:

```csharp
diverter.ResetAll();
Console.WriteLine(foo.Message); // "Original"
```

### Diverter registration builder

As you may not wish to replace the entire `ServiceCollection` a builder extension method is provided with various helper methods to select which registrations
are to be included or excluded. As the `ServiceCollection` registrations are stored in the order added, ranges of 
types can be selected. When using ranges only supported types are selected, i.e. interfaces and closed generics.
```csharp
services.Divert(diverter, builder =>
{
    builder.IncludeRange<IStart, IEnd>(); // include all registrations between IStart and IEnd (inclusive by default)
    builder.IncludeUntil<IStop>(inclusive:false); // include all from the start until IStop (not inclusive)
    builder.ExcludeFrom<IBoring>(); // exclude from IBoring to the end (inclusive by default)
    builder.Include<ILogger>(); // specifically include ILogger only
    builder.Exclude(typeof(INope)); // specifically exclude INope
});
```

### Open generic registrations
IServiceCollection supports open generic registrations for example:
services.AddTransient(typeof(IBar<>), typeof(Bar<>);

It is not possible to DivertR let



