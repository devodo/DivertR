---
layout: default
title: Named Arguments
nav_order: 4
parent: Verifying Calls
---

# Named Arguments

The `Verify` methods allows specifying call argument types and names using the same Via [`ValueTuple` syntax](#named-arguments):

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Record();

var result = fooProxy.Echo("record example");

nameCalls.Verify<(string input, __)>(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example echo");
}).Count.ShouldBe(1);
```

The argument `ValueTuple` can also be defined on the `Record` method and gets passed through to `Verify` calls:

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Record<(string input, __)>();

var result = fooProxy.Echo("record example");

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example echo");
}).Count.ShouldBe(1);
```

Finally if the argument `ValueTuple` is defined on a chained `Via` the strongly typed argument information is passed through to the `Record` and can be used in the `Verify` calls:

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(call => $"{call.Args.input} viaed")
    .Record();

var result = fooProxy.Echo("record example");

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example viaed");
}).Count.ShouldBe(1);
```
