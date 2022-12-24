---
layout: default
title: Retarget
nav_order: 7
parent: Redirects
---

# Retarget

A Redirect can be configured to *retarget* its proxy calls to a substitute instance of its target type:

```csharp
var fooRedirect = new Redirect<IFoo>();
var fooProxy = fooRedirect.Proxy(new Foo("MrFoo"));
Console.WriteLine(fooProxy.Name); // "MrFoo"

var fooTwo = new Foo("two");
Console.WriteLine(fooTwo.Name); // "two"

fooRedirect.Retarget(fooTwo);

Console.WriteLine(fooProxy.Name); // "two"
```

The retarget substitute can be any instance of the Redirect's target type including e.g. Mock objects:

```csharp
var mock = new Mock<IFoo>();
mock
    .Setup(x => x.Echo(It.IsAny<string>()))
    .Returns((string input) => $"{input} mock");

fooRedirect.Retarget(mock.Object);

Console.WriteLine(fooProxy.Echo("hello"));  // "hello mock"
```

When a `Retarget` is added it is also pushed onto the Redirect Via stack.
Retarget substitutes are also able to relay calls to the proxy root or next Via from the Redirect's `Relay` property:

```csharp
IFoo next = fooRedirect.Relay.Next;
IFoo root = fooRedirect.Relay.Root;
mock
    .Setup(x => x.Name)
    .Returns(() => $"{root.Name} {next.Name} mock");

Console.WriteLine(fooProxy.Name);  // "MrFoo two mock"
```
