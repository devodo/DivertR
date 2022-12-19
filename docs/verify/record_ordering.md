---
layout: default
title: Record Ordering
nav_order: 7
parent: Verifying Calls
---

# Record ordering

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo());

var fooCalls = fooRedirect.Record(opt => opt.OrderFirst());

fooRedirect
    .To(x => x.Echo(Is<string>.Any))
    .Via<(string input, __)>(call => call.CallNext() + " viaed")

fooProxy.Echo("record");

fooCalls
    .To(x => x.Echo(Is<string>.Any))
    .Verify<(string input, __)>(call =>
    {
        call.Args.input.ShouldBe("record");
        call.Returned.Value.ShouldBe("record viaed");
    }).Count.ShouldBe(1);
```
