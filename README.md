# DivertR

[![nuget](https://img.shields.io/nuget/v/DivertR.svg)](https://www.nuget.org/packages/DivertR)
[![build](https://github.com/devodo/DivertR/actions/workflows/build.yml/badge.svg)](https://github.com/devodo/DivertR/actions/workflows/build.yml)

DivertR is a .NET proxy framework that can be used to create test doubles.
It is similar to existing mocking frameworks like [Moq](https://github.com/moq/moq4) but provides additional features for integration testing of wired-up systems.

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

DivertR works by decorating dependency injection services with proxies that behave the same as the originals but that can be manipulated and reset dynamically.
This facilitate a style of testing where you start with the integrated, wired-up system and only mock out specific parts required per test.

For example, it can significantly speed up integration tests running against a [WebApplicationFactory (TestServer)](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests) instance by altering
and mocking dependencies between tests without requiring reinitialisation like this:

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
> The source code for the example above is available [here](./examples/DivertR.Examples.WebAppTests).
> 
> Follow the [Resources](#resources) section below for more examples, quickstart, documentation, etc. 

# Resources

* [Quickstart guide](https://devodo.github.io/DivertR/quickstart/)
* [Documentation](https://devodo.github.io/DivertR/)
* [Discussion](https://github.com/devodo/DivertR/discussions/43) - Feedback and comments are welcome
* [DivertR NuGet package](https://www.nuget.org/packages/DivertR)


