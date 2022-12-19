---
layout: default
title: Verify Visitor
nav_order: 3
parent: Verifying Calls
---

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
