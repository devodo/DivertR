# DivertR

.NET Dependency Injection Diversion

DivertR is a tool that facilitates an integrated approach to testing by making it easy to hotswap
test code in and out at the dependency injection layer.

With DivertR you can modify your dependency injection services at runtime by replacing them with configurable proxies.
These can redirect calls to test doubles, such as substitute instances or delegates, and then optionally relay back to the
original services. Update and reset proxy configurations, on the fly, while the process is running.

![DivertR Via](./docs/assets/images/DivertR_Via.svg)

## Quickstart

### Start with a Foo
Given an `IFoo` interface and a `Foo` implementation:

```csharp
public interface IFoo
{
    public string Name { get; set; }
    string Echo(string input);
}

public class Foo : IFoo
{
    public string Name { get; set; } = "original";
    
    public string Echo(string input)
    {
        return $"{Name}: {input}";
    }
}
```

With the following .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection` registration:

```csharp
IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo, Foo>();
```

### Hello DivertR
DivertR is installed into the `IServiceCollection` by decorating the existing `IFoo` registration using a provided extension method:

```csharp
var diverter = new Diverter();
services.Divert<IFoo>(diverter);
```

The `IServiceCollection` can now be used as usual to build the service provider and resolve dependency instances:

```csharp
IServiceProvider provider = services.BuildServiceProvider();
IFoo foo = provider.GetService<IFoo>();
foo.Name = "Foo1"

Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello"
```

### Redirect
At this stage the behaviour of the resolved `IFoo` instances is unchanged. However, it can be modified using 
a DivertR entity called a `Via` to configure a *redirect*:

```csharp
IVia<IFoo> fooVia = diverter.Via<IFoo>();
fooVia
    .Redirect(x => x.Echo(Is<string>.Any)) // (1)
    .To((string input) => $"{input} DivertR");   // (2)
  
Console.WriteLine(foo.Echo("Hello")); // "Hello DivertR"
```

The `Via` intercepts calls to the resolved `IFoo` instances.
By default calls are simply forwarded to the original registration, in this case instances of the `Foo` class.
However, after adding the redirect any calls that match the lambda expression (1) are redirected to the delegate (2).

The redirect is applied to all existing and future resolved `IFoo` instances:

```csharp
IFoo foo2 = provider.GetService<IFoo>();
foo2.Name = "Foo2";

Console.WriteLine(foo2.Echo("Hello")); // "Hello DivertR"
```

### Reset

To reset resolved instances back to their original behaviour simply discard all redirects on the `Via` with the following call:

```csharp
fooVia.Reset();
  
Console.WriteLine(foo.Echo("Hello"));  // "Foo1: Hello"
Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello"
```

So far we have only been working with a single `Via` instance, i.e. `IVia<IFoo>` bound to the `IFoo` registration type.
However, testing a system would typically require using multiple `Vias` for different types.
These can all be reset at once by calling: 

```csharp
diverter.ResetAll();
```

### Relay

The `Via` also lets you *relay* back to the original registration
by providing the `Relay.Next` property that can be called from the body of the redirect:

```csharp
IFoo next = fooVia.Relay.Next;
fooVia
    .Redirect(x => x.Echo(Is<string>.Any))
    .To((string input) =>
    {
        // run test code before
        // ...

        // call original instance
        var message = next.Echo(input);
    
        // run test code after
        // ...
    
        return $"{message} - Redirected";
    });
  
Console.WriteLine(foo.Echo("Hello"));  // "Foo1: Hello  - Redirected"
Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello  - Redirected"
```

> The `Relay.Next` property is a proxy that the `Via` connects to the current intercepted call.
> Its members can only be accessed within this context otherwise a `DiverterException` is thrown.

### Retarget

As well as redirecting to delegates you can also redirect to substitute targets. A valid 
substitute is anything that implements the target interface (in this case `IFoo`).
This includes, for example, Mock objects:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Echo(It.IsAny<string>()))
    .Returns((string input) => $"{next.Echo(input)} - Mocked");

fooVia
    .Redirect() // No parameter defaults to match all calls
    .To(mock.Object);

Console.WriteLine(foo.Echo("Hello"));  // "Foo1: Hello - Redirected - Mocked"
Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Redirected - Mocked"
```

Note the Mock also calls the `Relay.Next` property. However, it does not relay to the original registration as before.
Instead it goes to the delegate redirect that was previously added.
This is because adding a new redirect does not replace the existing ones. Instead they are pushed onto a stack
that the `Relay.Next` property traverses...

### Redirect chain

![Redirect Stack](./docs/assets/images/Redirect_Stack.svg)

When redirects are added they are pushed onto a stack. When the `Via` intercepts a call
it traverses down the stack, starting from the last redirect added. The call is passed to the first eligible redirect (e.g. its lambda expression constraint matches).
The redirect is then responsible for executing the call and can optionally continue back down the stack by calling the `Relay.Next` property. This will again traverse the stack
until the next matching redirect is found. When there are no more redirects the original instance is called.
> In summary, the redirects are stacked forming a chain of responsibility pipeline that is
> traversed with the `Relay.Next` property.

### Original relay

The `Via` also provides the `Relay.Original` property that relays directly to the original instance,
skipping over any remaining redirects.

```csharp
IFoo original = fooVia.Relay.Original;
fooVia
    .Redirect(x => x.Echo(Is<string>.Any))
    .To((string input) => $"{original.Echo(input)} - Skipped");
  
Console.WriteLine(foo.Echo("Hello"));  // "Foo1: Hello - Skipped"
Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Skipped"
```

> Similar to the `Relay.Next` property, `Relay.Original` is a proxy interface that relays to the original instance
> but its members can only be accessed within the context of a `Via` intercepted call.

### Class support

By default DivertR can only be used to redirect interfaces. `Via` and `Relay` proxies are generated using `System.Reflection.DispatchProxy`
that only supports interfaces.

It is possible to configure a different proxy factory, e.g. Castle Dynamic Proxy, that does support classes.
This is discouraged and is prone to unintuitive behaviour as only virtual or abstract class members can be intercepted and relayed.

### Async support

Task and ValueTask async calls are fully supported, e.g. if `IFoo` is extended to include an async method:

```csharp
public interface IFoo
{
    Task<string> EchoAsync(string input);
}

public class Foo : IFoo
{
    public async Task<string> EchoAsync(string input)
    {
        await Task.Yield();
        return $"{Name}: {input}";
    }
}

diverter.ResetAll(); // Discard all previous redirects

fooVia
    .Redirect(x => x.EchoAsync(Is<string>.Any))
    .To(async (string input) => $"{await next.EchoAsync(input)} - Async");

Console.WriteLine(await foo.EchoAsync("Hello"));  // "Foo1: Hello - Async"
Console.WriteLine(await foo2.EchoAsync("Hello")); // "Foo2: Hello - Async"
```
