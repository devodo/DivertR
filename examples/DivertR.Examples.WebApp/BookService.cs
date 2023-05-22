namespace DivertR.Examples.WebApp;

public class BookService : IBookService
{
    public Task<Book?> GetBookAsync(Guid id)
    {
        // Skipping the real implementation as this not required for the example.
        // Return null to simulate not found.
        return Task.FromResult<Book?>(null);
    }
}