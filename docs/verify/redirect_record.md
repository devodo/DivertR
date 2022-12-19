---
layout: default
title: Redirect Record
nav_order: 6
parent: Verifying Calls
---

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
