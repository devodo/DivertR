using DivertR.DependencyInjection;
using DivertR.Examples.WebApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace DivertR.Examples.WebAppTests;

public class ExampleFixture
{
    // Build a Diverter instance and register the DI services we want to be able to redirect
    private readonly IDiverter _diverter = new DiverterBuilder()
        .Register<IBookService>()
        .Create();

    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    
    public ExampleFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Install Diverter into the IServiceCollection
                services.Divert(_diverter);
            });
        });
    }

    public IDiverter InitDiverter()
    {
        // Reset all Diverter proxies
        return _diverter.ResetAll();
    }

    public HttpClient CreateHttpClient()
    {
        return _webApplicationFactory.CreateClient();
    }
}