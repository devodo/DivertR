# Developer guide

## Introduction

DivertR is a proxy interception framework built on top of DispatchProxy that
Similar to mocking frameworks like Moq lets you manipulate Dependency Injection
services by decorating registrations with configurable proxies. These proxies
share a lot of similarities 

In process integration and component testing 

Note the Mock also calls the `Relay.Next` property. However, it does not relay to the original registration as before.
Instead it goes to the delegate redirect that was previously added.
This is because adding a new redirect does not replace the existing ones. Instead they are pushed onto a stack
that the `Relay.Next` property traverses...

### Redirect chain

![Redirect Stack](./assets/images/Redirect_Stack.svg)

When redirects are added they are pushed onto a stack. When the `Via` intercepts a call
it traverses down the stack, starting from the last redirect added. The call is passed to the first eligible redirect (e.g. its lambda expression constraint matches).
The redirect is then responsible for executing the call and can optionally continue back down the stack by calling the `Relay.Next` property. This will again traverse the stack
until the next matching redirect is found. When there are no more redirects the original instance is called.
> In summary, the redirects are stacked forming a chain of responsibility pipeline that is
> traversed with the `Relay.Next` property.

### Original relay

The `Via` also provides the `Relay.Original` property that relays directly to the original instance,
skipping over any remaining redirects.

```csharp
IFoo original = fooVia.Relay.Original;
fooVia
    .To(x => x.Echo(Is<string>.Any))
    .Redirect((string input) => $"{original.Echo(input)} - Skipped");
  
Console.WriteLine(foo.Echo("Hello"));  // "Foo: Hello - Skipped"
Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Skipped"
```

> Similar to the `Relay.Next` property, `Relay.Original` is a proxy interface that relays to the original instance
> but its members can only be accessed within the context of a `Via` intercepted call.

### Open generic registrations
IServiceCollection supports open generic registrations for example:
services.AddTransient(typeof(IBar<>), typeof(Bar<>);

It is not possible to DivertR let



