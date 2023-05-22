using System.Net;
using System.Net.Http.Json;
using DivertR.Examples.WebApp;
using Xunit;

namespace DivertR.Examples.WebAppTests;

public class ExampleTests : IClassFixture<ExampleFixture>
{
    private readonly IDiverter _diverter;
    private readonly HttpClient _httpClient;

    public ExampleTests(ExampleFixture exampleFixture)
    {
        _diverter = exampleFixture.InitDiverter();
        _httpClient = exampleFixture.CreateHttpClient();
    }

    [Fact]
    public async Task GivenBookExists_WhenGetBookById_ThenReturnsBook()
    {
        // ARRANGE
        var mockedBook = new Book { Id = Guid.NewGuid(), Name = "Test Book" };
        
        // Configure IBookService to return a mocked result
        _diverter
            .Redirect<IBookService>() // Redirect IFooService calls 
            .To(x => x.GetBookAsync(Is<Guid>.Any)) // matching this method and argument value
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
}