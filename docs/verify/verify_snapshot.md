---
layout: default
title: Verify Snapshot
nav_order: 2
parent: Verifying Calls
---

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
~~~~