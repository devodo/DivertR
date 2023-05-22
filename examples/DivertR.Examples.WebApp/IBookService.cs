namespace DivertR.Examples.WebApp;

public interface IBookService
{
    Task<Book?> GetBookAsync(Guid id);
}