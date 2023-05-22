using DivertR.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IFooRepository, FooRepository>();
builder.Services.AddScoped<IFooService, FooService>();
builder.Services.AddSingleton<IBarServiceFactory, BarServiceFactory>();

var app = builder.Build();

app.MapControllers();
app.Run();