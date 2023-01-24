---
layout: default
title: Recording and Verifying
nav_order: 3
parent: Documentation
---

# Recording and Verifying

{: .no_toc }

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

DivertR can record the details of calls to its proxies and this can be used for test spying and verification.

# Record

The Redirect fluent interface is used to start a recording of proxy calls that match a `To` expression:

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo());

var echoCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any)) // Call match expression
    .Record(); // Returns an ICallStream collection of all recorded calls

var result = fooProxy.Echo("record test");
Console.WriteLine(result); // "record test"

// ICallStream is an `IReadOnlyCollection`
Console.WriteLine(echoCalls.Count); // 1

for(var call in echoCalls)
{
    Console.WriteLine(call.Args[0]); // "record test"
    Console.WriteLine(call.Returned.Value); // "record test"
}
```

The `Record` method returns an `ICallStream` that recorded called are appended to. This variable is an `IReadOnlyCollection` and `IEnumerable` that can be enumerated or queried e.g. with standard Linq expressions.

# Verify snapshot

The `ICallStream` interface provides `Verify` helper methods to facilitate iteration and verification over the recorded calls collection.

Recorded calls are appended to the `ICallStream` whenever a matching proxy call is made. This means the `ICallStream` will hold different record data at different points in time as calls are happening.

Therefore the `Verify` method iterates over and returns an immutable snapshot of the collection of recorded calls at the point of time it is called.
This allows performing multiple, consistent operations on a stable set of data. E.g. in the example above iterating over the collection and then verifying the call count.

```csharp
var echoCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Record();

fooProxy.Echo("one");
Console.WriteLine(echoCalls.Count); // 1

// Create verify snapshot
var verifyCalls = echoCalls.Verify();

// Call recorded method again
fooProxy.Echo("two");

// Record call stream has both calls
Console.WriteLine(echoCalls.Count); // 2

// Snapshot is immutable 
Console.WriteLine(verifyCalls.Count); // 1
Console.WriteLine(verifyCalls[0].Args[0]); // one
```

# Verify Visitor

The `ICallStream` interface provides `Verify` helper methods to facilitate iteration and verification over the recorded calls collection.

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo());

var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any)) 
    .Record();

var result = fooProxy.Echo("record");

var verifySnapshot = nameCalls.Verify(call =>
{
    call.Args[0].ShouldBe("record");
    call.Returned.Value.ShouldBe("record");
});

verifySnapshot.Count.ShouldBe(1); // The verify snapshot records calls at a point in time and is immutable
```

# Record chaining

The Redirect fluent interface allows chaining the `Record` method after a `Via` call:

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => call.CallNext() + " redirected")
    .Record();

var result = fooProxy.Echo("record");

nameCalls.Verify(call =>
{
    call.Args[0].ShouldBe("record");
    call.Returned.Value.ShouldBe("record redirected");
}).Count.ShouldBe(1);
```

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
    .Via<(string input, __)>(call => $"{call.Args.input} redirected")
    .Record();

var result = fooProxy.Echo("record example");

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example redirected");
}).Count.ShouldBe(1);
```

# Recording exceptions

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(() => throw new Exception())
    .Record();

Exception caughtException = null;
try
{
    fooProxy.Echo("record example");
}
catch (Exception ex)
{
    caughtException = ex;
}

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Exception.ShouldBe(caughtException)
    call.Returned.Value.ShouldBeNull();
}).Count.ShouldBe(1);
```

# Redirect Record

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo());

// Record all Redirect proxy calls
var fooCalls = fooRedirect.Record();

fooProxy.Echo("record");

fooCalls
    .To(x => x.Echo(Is<string>.Any)) // Use the 'To' expression to filter Redirect recorded calls 
    .Verify<(string input, __)>(call =>
    {
        call.Args.input.ShouldBe("record");
        call.Returned.Value.ShouldBe("record");
    }).Count.ShouldBe(1);
```

# Record ordering

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo());

var fooCalls = fooRedirect.Record(opt => opt.OrderFirst());

fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(call => call.CallNext() + " redirected")

fooProxy.Echo("record");

fooCalls
    .To(x => x.Echo(Is<string>.Any))
    .Verify<(string input, __)>(call =>
    {
        call.Args.input.ShouldBe("record");
        call.Returned.Value.ShouldBe("record redirected");
    }).Count.ShouldBe(1);
```