# DivertR

[![nuget](https://img.shields.io/nuget/v/DivertR.svg)](https://www.nuget.org/packages/DivertR)
[![build](https://github.com/devodo/DivertR/actions/workflows/build.yml/badge.svg)](https://github.com/devodo/DivertR/actions/workflows/build.yml)

DivertR is a general purpose .NET proxy framework that can be used to create test doubles.
It is similar to existing mocking frameworks like [Moq](https://github.com/moq/moq4) but is designed to work seamlessly with the dependency injection container by converting existing services into test friendly, configurable proxies. This facilitates an integrated style of testing where you start with the wired-up system and then mock out specific parts required per test.

# Installing

Install DivertR as a [NuGet package](https://www.nuget.org/packages/DivertR):

```sh
Install-Package DivertR
```

Or via the .NET command line interface:

```sh
dotnet add package DivertR
```

# Why?

The original motivation for creating DivertR was to be able to significantly speed up integration tests running against a [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests) instance by easily altering and mocking dependencies on a per test basis without requiring reinitialisation.

WebApplicationFactory does let you customise dependency injection services but this can only be done upfront before starting the test server instance.
To have different customisations between tests requires reinitialising a new test server instance each time.
This can be very slow when running many tests or larger applications with heavier startup.

# WebApplicationFactory Example

DivertR turns dependency injection services into configurable proxies that can be reconfigured between tests running against the same test server instance like this:

```csharp
[Fact]
public async Task GivenBookExists_WhenGetBookById_ThenReturnsBook()
{
    // ARRANGE
    var mockedBook = new Book { Id = Guid.NewGuid(), Name = "Test Book" };
    
    // Configure IBookService to return a mocked result
    _diverter
        .Redirect<IBookService>() // Redirect IFooService calls 
        .To(x => x.GetBookAsync(Is<Guid>.Any)) // matching this method and any argument value
        .Via(() => Task.FromResult(mockedBook)); // via this delegate
    
    // ACT
    var bookResult = await _httpClient.GetFromJsonAsync<Book>($"/books/{mockedBook.Id}");
    
    // ASSERT
    Assert.NotNull(bookResult);
    Assert.Equal(mockedBook.Id, bookResult.Id);
    Assert.Equal(mockedBook.Name, bookResult.Name);
}

[Fact]
public async Task GivenBookServiceError_WhenGetBookById_ThenReturns500InternalServerError()
{
    // ARRANGE
    var bookId = Guid.NewGuid();
    
    // Configure IBookService to throw an exception
    _diverter
        .Redirect<IBookService>()
        .To(x => x.GetBookAsync(bookId)) // match on bookId value only
        .Via(() => throw new Exception("Test"));
    
    // ACT
    var controlResponse = await _httpClient.GetAsync($"/books/{Guid.NewGuid()}");
    var testResponse = await _httpClient.GetAsync($"/books/{bookId}");
    
    
    // ASSERT
    Assert.Equal(HttpStatusCode.NotFound, controlResponse.StatusCode);
    Assert.Equal(HttpStatusCode.InternalServerError, testResponse.StatusCode);
}
```

> **Note**  
> The source code for the example above is available [here](https://github.com/devodo/DivertR/tree/main/examples/DivertR.Examples.WebAppTests).

# Mocking Example

DivertR is a general purpose framework that can be used in many different scenarios including for standard unit test mocking purposes.

```csharp
IFoo fooMock = Spy.On<IFoo>();

Spy.Of(fooMock)
    .To(x => x.Name)
    .Via(() => "redirected");

Assert.Equal("redirected", fooMock.Name);
```

# Documentation and Resources

Please follow the links below for more examples, quickstart, documentation, etc.

* [Quickstart guide](https://devodo.github.io/DivertR/quickstart/)
* [Documentation](https://devodo.github.io/DivertR/)
* [Discussion](https://github.com/devodo/DivertR/discussions/43) - Feedback and comments are welcome
* [DivertR NuGet package](https://www.nuget.org/packages/DivertR)


