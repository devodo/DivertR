# DivertR

[![nuget](https://img.shields.io/nuget/v/DivertR.svg)](https://www.nuget.org/packages/DivertR)
[![build](https://github.com/devodo/DivertR/actions/workflows/build.yml/badge.svg)](https://github.com/devodo/DivertR/actions/workflows/build.yml)

DivertR is a .NET library for creating proxy test doubles such as mocks, fakes and spies.
It is similar to mocking frameworks like the well known [Moq](https://github.com/moq/moq4) but provides, in addition, features for ***integration*** and ***component*** testing of wired-up systems.

# Installing

Install DivertR as a [NuGet package](https://www.nuget.org/packages/DivertR):

```sh
Install-Package DivertR
```

Or redirect the .NET command line interface:

```sh
dotnet add package DivertR
```

# Feature Summary

1. Test double proxy framework for mocking, faking, stubbing, spying, etc. [[more]](./docs/Redirect.md)
2. Proxies that wrap and forward to root (original) instances so tests run against the integrated system whilst modifying and spying on specific parts as needed. [[more]](./docs/Redirect.md#proxy)
3. A lightweight, fluent interface for configuring proxies to via calls to delegates or substitute instances. [[more]](./docs/Redirect.md#via)
4. Dynamic update and reset of proxies in a running application enabling changes between tests without requiring restart and initialisation overhead. [[more]](./docs/Redirect.md#reset)
5. Leveraging .NET ValueTuple types for specifying named and strongly typed call arguments that can be passed and reused e.g. in call verifications. [[more]](./docs/Redirect.md#named-arguments)
6. Method call interception and diversion with optional relay back to the original target. [[more]](./docs/Redirect.md#relay)
7. Simple plugging of proxy factories into the dependency injection container by decorating and wrapping existing registrations. [[more]](./docs/DI.md#redirect-registration)
8. Recording and verifying proxy calls. [[more]](./docs/Verify.md)

# Example Usage

DivertR can facilitate a style of testing where you start with a fully DI wired-up system and mock out specific parts per test.
For example, it can be used to write tests on a WebApp like this:

```csharp
[Fact]
public async Task GivenFooExistsInRepo_WhenGetFoo_ThenReturnsFoo_WithOk200()
{
    // ARRANGE
    var foo = new Foo
    {
        Id = Guid.NewGuid(),
        Name = "Foo123"
    };

    _diverter
        .Redirect<IFooRepository>() // Redirect IFooRepository calls 
        .To(x => x.GetFooAsync(foo.Id)) // matching this method and argument
        .Via(() => Task.FromResult(foo)); // redirect this delegate

    // ACT
    var response = await _fooClient.GetFooAsync(foo.Id);
    
    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.OK);
    response.Content.Id.ShouldBe(foo.Id);
    response.Content.Name.ShouldBe(foo.Name);
}

[Fact]
public async Task GivenFooRepoException_WhenGetFoo_ThenReturns500InternalServerError()
{
    // ARRANGE
    _diverter
        .Redirect<IFooRepository>()
        .To(x => x.GetFooAsync(Is<Guid>.Any))
        .Via(() => throw new Exception());

    // ACT
    var response = await _fooClient.GetFooAsync(Guid.NewGuid());

    // ASSERT
    response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
}
```

# Quickstart

For more examples and a demonstration of setting up a test harness for a WebApp see this [WebApp Testing Sample](./test/DivertR.WebAppTests/WebAppTests.cs)
or follow below for a quickstart:

* [Redirects](./docs/Redirect.md) for creating and configuring proxies.
* [Dependency Injection](./docs/DI.md) integration.
* [Recording and Verifying](./docs/Verify.md) calls.
* [About](./docs/About.md) DivertR.
