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

Or via the .NET command line interface:

```sh
dotnet add package DivertR
```

# Example Usage

DivertR can facilitate a style of testing where you start with a dependency injection wired-up system and mock out specific parts per test.
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
        .Via(() => Task.FromResult(foo)); // via this delegate

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

# Resources

* [Documentation and quickstart guide](https://devodo.github.io/DivertR/)
* [Discussion](https://github.com/devodo/DivertR/discussions/43) - Feedback and comments are welcome
* [DivertR NuGet package](https://www.nuget.org/packages/DivertR)


