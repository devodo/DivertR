# Via

Vias are the main DivertR entities used to create and configure proxies.
`Via` instances are instantiated from the generic `Via<TTarget>` class:

```csharp
IVia<IFoo> fooVia = new Via<IFoo>();
```

# Proxy

A `Via` creates proxy objects of its generic `TTarget` type.
E.g. an `IVia<IFoo>` like the one instantiated above creates `IFoo` proxies:

```csharp
var fooVia = new Via<IFoo>();

IFoo fooProxy = fooVia.Proxy(); // Create a proxy
IFoo fooTwo = fooVia.Proxy(); // Create another proxy
```

> A single Via can create any number of proxies. 

## Proxy Root

When a proxy object is created it can be given a *root* instance of its target type.
The default behaviour of a proxy is to relay all calls to its root:

```csharp
IFoo fooRoot = new Foo("MrFoo");
Console.WriteLine(fooRoot.Name); // "MrFoo"

var fooVia = new Via<IFoo>();
IFoo fooProxy = fooVia.Proxy(fooRoot);
Console.WriteLine(fooProxy.Name); // "MrFoo"
```

When a proxy is in its initial state it is transparent, i.e. it behaves identically to its root instance. Therefore if a system has its instances replaced with transparent proxies its original behaviour is left unchanged.

## Dummy Root

If no root instance is provided then the proxy is created with a *dummy* root that returns default values:

```csharp
var fooVia = new Via<IFoo>();

var fooMock = fooVia.Proxy(); // Proxy created with dummy root
Console.WriteLine(fooMock.Name); // null
```

In general dummy roots return the .NET `default` of the call's return type, e.g. `null` for reference types and `0` for `int`.
There are some special cases such as `Task` types are returned as `null` valued completed Tasks.
> Proxies with dummy roots can be used as mock objects.

# Redirect

`Redirect` instances are added to a `Via` to control the way its proxies behave.
Proxy calls are diverted and passed to the redirect for handling.
A fluent interface is provided on the Via for building and adding redirects to itself:

```csharp
var foo = new Foo("MrFoo");
Console.WriteLine(foo.Name); // "MrFoo"

var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(foo);
Console.WriteLine(fooProxy.Name); // "MrFoo"

// Add a redirect to the Via
fooVia
    .To(x => x.Name)                   // 1. Match expression
    .Redirect(() => "Hello Redirect"); // 2. Redirect delegate

Console.WriteLine(fooProxy.Name); // "Hello Redirect"
```

The redirect intercepts any proxy calls matching the `To` expression 1. and diverts them to the `Redirect` delegate 2.

Redirects can be added to a Via at any time and apply immediately to all its existing proxies as well as any created afterwards.

## Parameter matching

If the redirect `To` expression specifies a method with parameters, these are matched to call arguments as follows:

```csharp
// Match calls to the Echo method with any argument value
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

## Call arguments

Proxy call arguments can be passed to the redirect delegate as follows:

```csharp
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect(call => $"{call.Args[0]} redirected");
  
Console.WriteLine(fooProxy.Echo("me")); // "me redirected"
```
> The `Args` property is an `IReadOnlyList<object>` collection. 

## Named arguments

Strongly typed and named arguments can be specified by defining a `ValueTuple` generic type on the `Redirect` method as follows:

```csharp
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect<(string input, __)>(call => $"{call.Args.input} redirected");

Console.WriteLine(fooProxy.Echo("me")); // "me redirected"
```

Call arguments are mapped in parameter order onto the `ValueTuple` items and it replaces the `Args` property from the previous example.

The special Diverter type `__` (double underscore) is used to specify a discard mapping that is ignored so that only named types of parameters that will be used need to be defined.

C# requires named ValueTuples to have at least two parameters. If the call only has a single parameter, as in the example above,
then the discard type `__` must be used to provide a second dummy parameter.

## Relay root

The redirect delegate can *relay* calls back to the proxy root by calling the `Relay.Root` property:

```csharp
fooVia
    .To(x => x.Name)
    .Redirect(call =>
    {
        IFoo root = call.Relay.Root;
        return $"{root.Name} relayed";
    });

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo relayed"
```

> The `Relay.Root` property is a proxy that relays calls to the root instance.

## Relay next

![Redirect Stack](./assets/images/Redirect_Stack.svg)

Any number of redirects can be added to a Via. When redirects are added they are pushed onto a stack (with the last added at the top).
Proxy calls are traversed through the stack from top to bottom. If a call matches the `To` constraint it is passed to the redirect delegate for handling.
If no redirects match, the call falls through the stack to the root instance.

Redirect delegates can relay the call directly to the root as in the previous example
but they can also continue the call down the redirect stack by calling the `Relay.Next` property as follows:

```csharp
fooVia
    .To(x => x.Name)
    .Redirect(call => $"{call.Relay.Next.Name} 1")
    .Redirect(call => $"{call.Relay.Next.Name} 2") // Redirect calls can be chained
    .Redirect(call => $"{call.Next.Name} 3"); // The Root and Next properties can be accessed directly from the call argument

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1 2 3"
```

> The `Relay.Next` property is a proxy that relays calls to the next redirect that matches.
> If no redirects match it will relay to the root.
> The Root and Next properties can also be accessed directly from the call argument for convenience.

## Call forwarding

A redirect can call `CallRoot()` to forward the call to the target method of the root instance:

```csharp
fooVia
    .To(x => x.Name)
    .Redirect(call => call.CallRoot() + " 1");

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1"
```

Or the call can be forwarded down the redirect stack using `CallNext()`:

```csharp
fooVia
    .To(x => x.Name)
    .Redirect(call => call.CallNext() + " 1")
    .Redirect(call => call.CallNext() + " 2");

Console.WriteLine(fooRoot.Name); // "MrFoo"
Console.WriteLine(fooProxy.Name); // "MrFoo 1 2"
```

If the target method has parameters then `CallRoot()` and `CallNext()` forward the arguments from the original call. Custom arguments can be forwarded by passing an `object[]` to `CallRoot()` or `CallNext()`:

```csharp
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect(call => call.CallNext(new[] { "you" }));

Console.WriteLine(fooProxy.Echo("me")); // "you"
```

## Void methods

For redirect methods that return `void`, the same `Via` fluent interface syntax is used, only the delegate provided is an `Action` rather than a `Func`:

```csharp
fooVia
    .To(x => x.SetAge(Is<int>.Any))
    .Redirect<(int age, __)>(call =>
    {
        call.CallNext() + 10;
    });
```

## Property Getters and Setters

Redirects for property getters, [demonstrated earlier](#redirect), are added using the same `To` syntax as for standard methods. However, to indicate a redirect is for a property setter, the `ToSet` method is used instead:

```csharp
fooVia
    .ToSet(x => x.Name)
    .Redirect<(string name, __)>(call =>
    {
        call.Next.Name = call.Args.name + " changed";
    });
```

By default the redirect above will match any setter value input but the `ToSet` method accepts a second parameter as a value match expression using the usual [parameter matching](#parameter-matching) syntax:

```csharp
fooVia
    .ToSet(x => x.Name, () => Is<string>.Match(p => p.StartsWith("M")))
    .Redirect<(string name, __)>(call =>
    {
        call.Next.Name = name + " changed";
    });

fooProxy.Name = "Me";
Console.WriteLine(fooProxy.Name); // "Me changed"
```

## Async methods

Async is fully supported by DivertR and redirect delegates can be added to `Task` or `ValueTask` methods using the standard C# `async` syntax:

```csharp
fooVia
    .To(x => x.SaveAsync(Is<string>.Any, Is<CancellationToken>.Any))
    .Redirect(async call =>
    {
        var result = await call.CallNext();
        
        return result;
    });
```

## Throwing exceptions

Redirect delegates can throw exceptions using standard C# syntax and any exceptions thrown will bubble up to callers as usual:

```csharp
fooVia
    .To(x => x.Echo("exception"))
    .Redirect(() => throw new MyException())

fooProxy.Echo("exception"); // throws MyException
```

## Generic methods

```csharp
fooVia
    .To(x => x.Echo<int>(Is<int>.Any))
    .Redirect(call => call.CallNext() * 2);

```

# Reset

A Via can be *reset* which removes all its redirects, reverting its proxies to their original behaviour:

```csharp
fooVia.To(x => x.Name).Redirect("diverted");
Console.WriteLine(fooProxy.Name);  // "diverted"

fooVia.Reset();
  
Console.WriteLine(fooProxy.Name);  // "MrFoo"
```

The reset method on the Via can be called at any time and is applied immediately to all its proxies.
By adding redirects and resetting, proxy behaviour can be modified at runtime allowing a running process to be altered between tests, e.g. to avoid restart and initialisation overhead.

# Retarget

A Via can be configured to *retarget* its proxy calls to a substitute instance of its target type:

```csharp
var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(new Foo("MrFoo"));
Console.WriteLine(fooProxy.Name); // "MrFoo"

var fooTwo = new Foo("two");
Console.WriteLine(fooTwo.Name); // "two"

fooVia.Retarget(fooTwo);

Console.WriteLine(fooProxy.Name); // "two"
```

The retarget substitute can be any instance of the Via's target type including e.g. Mock objects:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Echo(It.IsAny<string>()))
    .Returns((string input) => $"{input} mock");

fooVia.Retarget(mock.Object);

Console.WriteLine(fooProxy.Echo("hello"));  // "hello mock"
```

When a `Retarget` is added it is also pushed onto the Via redirect stack.
Retarget substitutes are also able to relay calls to the proxy root or next redirect from the Via's `Relay` property:

```csharp
IFoo next = fooVia.Relay.Next;
IFoo root = fooVia.Relay.Root;
mock
    .Setup(x => x.Name)
    .Returns(() => $"{root.Name} {next.Name} mock");

Console.WriteLine(fooProxy.Name);  // "MrFoo two mock"
```

# Strict mode

Enable *strict mode* on a Via to ensure only methods with registered redirects are allowed to be called:

```csharp
fooVia.Strict(); // enables strict mode

fooVia
    .To(x => x.Echo("ok"))
    .Redirect(call => call.Args[0]);

fooProxy.Echo("me"); // throws StrictNotSatisfiedException
fooProxy.Echo("ok"); // "ok" 
```

When strict mode is enabled a `StrictNotSatisfiedException` is thrown if a call is made to a proxy and does not match any redirects.

Strict mode is disabled when a Via is created or [reset](#reset).
