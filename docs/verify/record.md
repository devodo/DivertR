---
layout: default
title: Record
nav_order: 1
parent: Verifying Calls
---

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
