# Recording and Verifying Calls

DivertR can record the details of calls to its proxies and this can be used for test spying and verification.

## Record

The Via fluent interface is used to start a recording of proxy calls that match a `To` expression:

```csharp
var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(new Foo());

var echoCalls = fooVia
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

## Verify

The `ICallStream` interface provides `Verify` helper methods to facilitate iteration and verification over the recorded calls collection.

```csharp
var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(new Foo());

var nameCalls = fooVia
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

### Verify snapshot

Recorded calls are appended to the `ICallStream` whenever a matching proxy call is made. This means the `ICallStream` may hold different record data at different points in time.
Therefore the `Verify` method iterates over and returns an immutable snapshot of the collection of recorded calls at the point of time it is called.
This allows performing multiple, consistent operations on a stable set of data. E.g. in the example above iterating over the collection and then verifying the call count.

```csharp
var echoCalls = fooVia
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

## Record chaining

The Via fluent interface allows chaining the `Record` method after a `Redirect` call:

```csharp
var nameCalls = fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect(call => call.CallNext() + " redirected")
    .Record();

var result = fooProxy.Echo("record");

nameCalls.Verify(call =>
{
    call.Args[0].ShouldBe("record");
    call.Returned.Value.ShouldBe("record redirected");
}).Count.ShouldBe(1);
```

## Verify named arguments

The `Verify` methods allows specifying call argument types and names using the same Redirect [`ValueTuple` syntax](#named-arguments):

```csharp
var nameCalls = fooVia
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
var nameCalls = fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Record<(string input, __)>();

var result = fooProxy.Echo("record example");

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example echo");
}).Count.ShouldBe(1);
```

Finally if the argument `ValueTuple` is defined on a chained `Redirect` the strongly typed argument information is passed through to the `Record` and can be used in the `Verify` calls:

```csharp
var nameCalls = fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect<(string input, __)>(call => $"{call.Args.input} redirected")
    .Record();

var result = fooProxy.Echo("record example");

nameCalls.Verify(call =>
{
    call.Args.input.ShouldBe("record example");
    call.Returned.Value.ShouldBe("record example redirected");
}).Count.ShouldBe(1);
```

## Recording exceptions

```csharp
var nameCalls = fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect<(string input, __)>(() => throw new Exception())
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

## Via Record

```csharp
var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(new Foo());

// Record all Via proxy calls
var fooCalls = fooVia.Record();

fooProxy.Echo("record");

fooCalls
    .To(x => x.Echo(Is<string>.Any)) // Use the 'To' expression to filter Via recorded calls 
    .Verify<(string input, __)>(call =>
    {
        call.Args.input.ShouldBe("record");
        call.Returned.Value.ShouldBe("record");
    }).Count.ShouldBe(1);
```

## Record ordering

```csharp
var fooVia = new Via<IFoo>();
var fooProxy = fooVia.Proxy(new Foo());

var fooCalls = fooVia.Record(opt => opt.OrderFirst());

fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect<(string input, __)>(call => call.CallNext() + " redirected")

fooProxy.Echo("record");

fooCalls
    .To(x => x.Echo(Is<string>.Any))
    .Verify<(string input, __)>(call =>
    {
        call.Args.input.ShouldBe("record");
        call.Returned.Value.ShouldBe("record redirected");
    }).Count.ShouldBe(1);
```
