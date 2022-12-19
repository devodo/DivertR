---
layout: default
title: Record Chaining
nav_order: 3
parent: Verifying Calls
---

# Record chaining

The Redirect fluent interface allows chaining the `Record` method after a `Via` call:

```csharp
var nameCalls = fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via(call => call.CallNext() + " viaed")
    .Record();

var result = fooProxy.Echo("record");

nameCalls.Verify(call =>
{
    call.Args[0].ShouldBe("record");
    call.Returned.Value.ShouldBe("record viaed");
}).Count.ShouldBe(1);
```
