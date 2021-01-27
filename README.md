# Divertr

The Dependency Injection Diverter

Divertr is a tool that facilitates a top-down approach to testing by allowing you to modify DI services at runtime.
Use it to replace your services with proxies that intercept calls and redirect them to substitute instances such as test doubles.
The proxy behaviour can be reconfigured and reset on the fly while your app is running.

## Create a proxy

Given a simple interface and implementation:

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

Then use it to create a proxy of an instance:

```csharp
var foo = new Foo {Message = "hello foo"};
var fooProxy = diverter.Proxy(foo);
Console.WriteLine(fooProxy.Message); // "hello foo"
```
> By default proxies simply direct calls to the original instance. 

## Configure a redirect

Modify the proxy behaviour to redirect calls to a different instance:

```csharp
diverter.Redirect(new Foo {Message = "hello Divertr"});
Console.WriteLine(fooProxy.Message); // "hello Divertr"
```

Then reset the proxy to its default behaviour:

```csharp
diverter.Reset();
Console.WriteLine(fooProxy.Message); // "hello foo"
```

## Redirect to a mock

You can configure proxies to redirect to test doubles such as mocks:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => "bye bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "bye bar"
```

## Reference the original instance

The redirected call can reference the original instance.
This allows you to intercept calls, enrich and inspect, and then continue the original execution flow:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{diverter.CallContext.Original.Message} bar");

diverter.Redirect(mock.Object);
Console.WriteLine(fooProxy.Message); // "hello foo bar"
```

## Divert multiple proxies

The configured redirects are applied to all proxies created from the same Divertr instance.
This allows you to divert calls from all instances of a given type to a single redirect instance.
This is useful, for example,  to decorate the behaviours of transient DI services where multiple instances of a type can be created.

```csharp
var fooA = diverter.Proxy(new Foo {Message = "foo A"});
var fooB = diverter.Proxy(new Foo { Message = "foo B" });

var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"Hello {diverter.CallContext.Original.Message}");

diverter.Redirect(mock.Object);

Console.WriteLine(fooA.Message); // "Hello foo A"
Console.WriteLine(fooB.Message); // "Hello foo B"
```

## Chain redirects

Multiple redirects can be appended together and referenced in the call allowing you to create a chain of responsibility pipeline.
The `CallContext.Replaced` property references the previous redirect in the chain.
> If only a single redirect is registered the `Replaced` and `Original` properties reference the same original instance.

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Message)
    .Returns(() => $"{diverter.CallContext.Replaced.Message} bar");

diverter
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object)
    .AddRedirect(mock.Object);

Console.WriteLine(fooProxy.Message); // "hello foo bar bar bar"
```
