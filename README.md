# DivertR

The .NET Dependency Injection Diverter

DivertR is a tool that facilitates an integrated, top-down approach to testing by making it easy to hotswap
test code in and out of a working system.

With DivertR you can modify your dependency injection services at runtime by replacing them with configurable proxies.
These can redirect calls to test doubles, such as substitute instances or delegates, and then optionally relay back to the original instances. 
Update and reset proxies, on the fly, while the process is running.

![DivertR Via](./docs/assets/images/DivertR_Via.svg)

## Quickstart

### Start with a Foo
Given an `IFoo` interface and its implementation registered with the standard .NET `Microsoft.Extensions.DependencyInjection.IServiceCollection`:

```csharp
public interface IFoo
{
    string GetMessage(string input);
}

public class Foo : IFoo
{
    public string GetMessage(string input)
    {
        return $"{input} original";
    }
}

IServiceCollection services = new ServiceCollection();
services.AddTransient<IFoo, Foo>();
```
### Hello DivertR
DivertR is installed into the `IServiceCollection` with a provided extension method:

```csharp
var diverter = new Diverter();
services.Divert(diverter);
```

The `IServiceCollection` can now be used as usual to build a provider and resolve dependency instances:

```csharp
IServiceProvider provider = services.BuildServiceProvider();
IFoo foo = provider.GetService<IFoo>();

Console.WriteLine(foo.GetMessage("Hello")); // "Hello original"
```

### Redirect
At this stage the behaviour of the resolved `IFoo` instance is unaltered, however it can now be modified using 
a DivertR entity called a `Via` to register a *redirect*:

```csharp
IVia<IFoo> fooVia = diverter.Via<IFoo>();
fooVia
  .Redirect(x => x.GetMessage(Is<string>.Any)) // (1)
  .To((string input) => $"{input} DivertR");   // (2)
  
Console.WriteLine(foo.GetMessage("Hello")); // "Hello DivertR"
```

The `Via` intercepts calls to the resolved `IFoo` instances.
By default calls simply go to the original dependency registration, in this case instances of the `Foo` class.
However, after adding the redirect any calls that match the lambda expression (1) are redirected to the delegate (2).

### Reset

To reset registrations back to their original behaviour simply discard all `Via` redirects with the following call:

```csharp
diverter.ResetAll();
  
Console.WriteLine(foo.GetMessage("Hello")); // "Hello original"
```

### Relay

The `Via` also lets you *relay* calls back to the original registration from the body of the redirect.
This feature allows you to inject test code and modifications around the original flow of execution:

```csharp
IFoo next = fooVia.Relay.Next;
fooVia
  .Redirect(x => x.GetMessage(Is<string>.Any))
  .To((string input) =>
  {
    // run test code before
    // ...

    // call original instance
    var original = next.GetMessage(input);
    
    // run test code after
    // ...
    
    return $"Redirected: {original}";
  });
  
Console.WriteLine(foo.GetMessage("Hello")); // "Redirected: Hello original"
```
> The `Relay.Next` property is a special proxy interface but its members can only be accessed
> within the context of a `Via` intercepted call. Access outside this context will
> result in a `DiverterException` being thrown.

