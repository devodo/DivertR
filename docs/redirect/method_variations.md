---
layout: default
title: Method Variations
nav_order: 6
parent: Redirect
---

# Method variations

## Async methods

Async is fully supported by DivertR and Via delegates can be added to `Task` or `ValueTask` methods using the standard C# `async` syntax:

```csharp
fooRedirect
    .To(x => x.SaveAsync(Is<string>.Any, Is<CancellationToken>.Any))
    .Via(async call =>
    {
        var result = await call.CallNext();
        
        return result;
    });
```

## Property Getters and Setters

Vias for property getters, [demonstrated earlier](#via), are added using the same `To` syntax as for standard methods. However, to indicate a Via is for a property setter, the `ToSet` method is used instead:

```csharp
fooRedirect
    .ToSet(x => x.Name)
    .Via<(string name, __)>(call =>
    {
        call.Next.Name = call.Args.name + " changed";
    });
```

By default the Via above will match any setter value input but the `ToSet` method accepts a second parameter as a value match expression using the usual [parameter matching](#parameter-matching) syntax:

```csharp
fooRedirect
    .ToSet(x => x.Name, () => Is<string>.Match(p => p.StartsWith("M")))
    .Via<(string name, __)>(call =>
    {
        call.Next.Name = name + " changed";
    });

fooProxy.Name = "Me";
Console.WriteLine(fooProxy.Name); // "Me changed"
```

## Void methods

For methods that return `void`, the same `Redirect` fluent interface syntax is used, only the `Via` delegate provided is an `Action` rather than a `Func`:

```csharp
fooRedirect
    .To(x => x.SetAge(Is<int>.Any))
    .Via<(int age, __)>(call =>
    {
        call.Next.SetAge(call.Args.ags + 10);
    });
```

## Generic methods

Generic method Vias are declared using the same fluent syntax ands are matched on the specified generic type arguments.

```csharp
fooRedirect
    .To(x => x.Echo<int>(Is<int>.Any))
    .Via(call => call.CallNext() * 2);
```

## Throwing exceptions

Via delegates can throw exceptions using standard C# syntax and any exceptions thrown will bubble up to callers as usual:

```csharp
fooRedirect
    .To(x => x.Echo("exception"))
    .Via(() => throw new MyException())

fooProxy.Echo("exception"); // throws MyException
```
