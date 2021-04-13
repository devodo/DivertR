# User guide

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



