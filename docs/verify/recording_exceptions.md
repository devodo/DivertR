---
layout: default
title: Recording Exceptions
nav_order: 5
parent: Verifying Calls
---

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
