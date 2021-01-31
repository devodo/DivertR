# Divertr

The Dependency Injection Diverter

Divertr is a testing tool that lets you modify your dependency injection services at runtime.
Use it to replace your DI services with proxies that intercept calls and redirect them to substitute instances such as test doubles.
The proxy behaviour can be reconfigured and reset on the fly while your app is running.

This facilitates testing a system as an integrated whole by making it easy to precisely control the behaviour of its
components (internal and external) and examine their interactions.

* [Quickstart](#quickstart)
    * [Create a Divertr proxy](#create-a-divertr-proxy)
    * [Configure a redirect](#configure-a-redirect)
    * [Redirect to a mock](#redirect-to-a-mock)
    * [Call the original instance](#call-the-original-instance)
    * [Divert multiple proxies](#divert-multiple-proxies)
    * [Chain redirects](#chain-redirects)
* [IServiceCollection Integration](#iserviceCollection-integration)
    * [Diverter Extensions](#diverter-extensions)
    * [Diverter Set Extensions](#diverter-set-extensions) 


## Quickstart
### Create a Divertr proxy

Given an IFoo interface and its implementation:

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
> By default proxies simply direct calls to the original instance. 

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
    .Returns(() => "bye bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "bye bar"
```

### Call the original instance

The redirected instance can call a proxy pointing to the originally targeted instance from the `CallCtx.Original` property.
This allows you, for example, to intercept calls, enrich and inspect, and then continue the original execution flow:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{diverter.CallCtx.Original.Message} bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "hello foo bar"
```

### Divert multiple proxies

The configured redirects are applied to all proxies created from the same Divertr instance.
This allows you to divert calls from all instances of a given type to a single redirect instance.
This is useful, for example,  to decorate the behaviours of transient DI services where multiple instances of a type can be created.

```csharp
var fooA = diverter.Proxy(new Foo {Message = "foo A"});
var fooB = diverter.Proxy(new Foo { Message = "foo B" });

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

### Diverter Set
```csharp
var diverterSet = new DiverterSet();
var diverter = diverterSet.Get<IFoo>();
var barDiverter = diverterSet.Get<IBar>();

diverterSet.ResetAll();
```

## IServiceCollection Integration
.NET `IServiceCollection`

### Diverter Extensions

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
```

```csharp
var diverter = new Diverter<IFoo>();
services.Divert(diverter);
```

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();
Console.WriteLine(foo.Message); // "Original"
```

```csharp
diverter.Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"

diverter.Reset();
Console.WriteLine(foo.Message); // "Original"
```

### Diverter Set Extensions

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo>(_ => new Foo {Message = "Original"});
```

```csharp
var diverterSet = new DiverterSet();
services.Divert(diverterSet);
```

```csharp
IServiceProvider provider = services.BuildServiceProvider();
var foo = provider.GetService<IFoo>();

diverterSet.Get<IFoo>().Redirect(new Foo {Message = "Diverted"});
Console.WriteLine(foo.Message); // "Diverted"
```

```csharp
diverterSet.ResetAll();
Console.WriteLine(foo.Message); // "Original"
```


