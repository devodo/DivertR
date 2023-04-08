using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddTransient<IFooRepository, FooRepository>();
builder.Services.AddScoped<IFooService, FooService>();
builder.Services.AddSingleton<IBarServiceFactory, BarServiceFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.MapControllers();
app.Run();