using DivertR.Examples.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IBookService, BookService>();

var app = builder.Build();

app.MapGet("/books/{id:guid}", async (Guid id, IBookService fooService) =>
{
    var book = await fooService.GetBookAsync(id);

    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.Run();