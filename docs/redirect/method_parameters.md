---
layout: default
title: Method Parameters
nav_order: 4
parent: Redirects
---

# Method parameters

Via intercept match rules can be configured on method parameters using call argument values and these can also be passed to Via delegates.

## Parameter matching

If the Via `To` expression specifies a method with parameters, these are matched to call arguments as follows:

```csharp
// Match calls to the Echo method with any argument value
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(() => "any");

// Match calls with arguments that satisfy a Match expression
fooRedirect
    .To(x => x.Echo(Is<string>.Match(a => a == "two")))
    .Via(() => "match");

// Match calls with arguments equal to a specified value
fooRedirect
    .To(x => x.Echo("three"))
    .Via(() => "equal");

Console.WriteLine(fooProxy.Echo("one")); // "any"
Console.WriteLine(fooProxy.Echo("two")); // "match"
Console.WriteLine(fooProxy.Echo("three")); // "equal"
```

## Call arguments

Proxy call arguments can be passed to the Via delegate as follows:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => $"{call.Args[0]} viaed");
  
Console.WriteLine(fooProxy.Echo("me")); // "me viaed"
```
> The `Args` property is an `IReadOnlyList<object>` collection.

## Named arguments

Strongly typed and named arguments can be specified by defining a `ValueTuple` generic type on the `Via` method as follows:

```csharp
fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(call => $"{call.Args.input} viaed");

Console.WriteLine(fooProxy.Echo("me")); // "me viaed"
```

Call arguments are mapped in parameter order onto the `ValueTuple` items and it replaces the `Args` property from the previous example.

The special Diverter type `__` (double underscore) is used to specify a discard mapping that is ignored so that only named types of parameters that will be used need to be defined.

C# requires named ValueTuples to have at least two parameters. If the call only has a single parameter, as in the example above,
then the discard type `__` must be used to provide a second dummy parameter.
