# Quickstart

## DivertR

DivertR is a framework for creating proxies that can be used as test doubles such as mocks for unit testing.

## Via

DivertR proxies are created and configured using `Via` instances.
These can be created by simply instantiating the `Via<TTarget>` class as follows:

```csharp
IVia<IFoo> fooVia = new Via<IFoo>();
```

## Proxies

A `Via` is used to create proxy objects of its target type.
E.g. the `IVia<IFoo>` instantiated above creates `IFoo` proxies as follows:

```csharp
IFoo fooProxy = fooVia.Proxy();
IFoo fooProxy2 = fooVia.Proxy();
// Note: A single Via instance can create any number of proxies.
```

### Proxy Root

When a proxy object is created it can be given a *root* instance of its type. By default all calls are forwarded (proxied) to the root.

```csharp
var fooRoot = new Foo();
var fooProxy = fooVia.Proxy(fooRoot);
Console.WriteLine(fooProxy.Name);
// fooProxy.Name == fooRoot.Name
```

### Dummy Root

If no root instance is provided then the proxy is created with a *dummy* root that returns default values.
Generally the dotnet default for the type is returned such as `null`
for reference types and `0` for `int`. There are some special cases such as `Task` types return `null` wrapped completed Tasks.
As proxies forward calls to their roots, calls to proxy members therefore return dummy values.

```csharp
var fooProxy = fooVia.Proxy(); // Proxy created with dummy root
Console.WriteLine(fooProxy.Name); // Returns null
```

## Redirect

The way proxies behave can be changed by adding *redirects* to the Via as follows:

```csharp
fooVia
    .To(x => x.Name)
    .Redirect(() => "Hello Redirect");
  
Console.WriteLine(fooProxy.Name); // "Hello Redirect"
```

Now any calls to the proxy matching the 'To' expression are diverted to the 'Redirect' delegate.
The redirects added are applied to all of the Via's existing proxies as well as any proxies created afterwards.

### Parameter matching

If the redirect 'To' expression method has parameters they are matched to call argument as follows:

```csharp
// Match calls with any argument value
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect(() => "any");

// Match calls with arguments that satisfy a Match expression
fooVia
    .To(x => x.Echo(Is<string>.Match(a => a == "two")))
    .Redirect(() => "match");

// Match calls with arguments equal to a specified value
fooVia
    .To(x => x.Echo("three"))
    .Redirect(() => "equal");

Console.WriteLine(fooProxy.Echo("one")); // "any"
Console.WriteLine(fooProxy.Echo("two")); // "match"
Console.WriteLine(fooProxy.Echo("three")); // "equal"
```

### Call arguments
```csharp
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect(call => $"{call.Args[0]} redirected");
  
Console.WriteLine(fooProxy.Echo("me")); // "me redirected"
```

### Named arguments

### Relay

## Record
