using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DivertR.DependencyInjection;
using DivertR.WebApp.Services;
using Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Refit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests.TestHarness
{
    public class WebAppFixture
    {
        // Build a Diverter instance
        private readonly IDiverter _diverter = new DiverterBuilder()
            // Register DI services we want to be able to redirect:
            .Register<IFooRepository>()
            .Register<IBarServiceFactory>()
            .Register<IFooService>()
            
            // Include standalone (non DI service) redirects:
            .IncludeRedirect<ITestOutputHelper>() // Add xUnit ITestOutputHelper logger
            
            // Add persistent redirect configurations:
            .Redirect<IBarServiceFactory>().ViaRedirect<IBarService>() // Redirect proxy IBarService instances created by IBarServiceFactory
            .Create();

        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    // Create an xUnit ITestOutputHelper proxy mock
                    var outputHelperMock = _diverter.Redirect<ITestOutputHelper>().Proxy();
                    // Add an xUnit logging provider that writes to the mock ITestOutputHelper
                    logging.AddXunit(outputHelperMock, options => options.IncludeScopes = true);
                });
                
                builder.ConfigureTestServices(services =>
                {
                    // Install DivertR into the IServiceCollection
                    services.Divert(_diverter);
                });
            });
        }

        public IServiceProvider Services => _webApplicationFactory.Services;

        public IDiverter InitDiverter(ITestOutputHelper? output = null)
        {
            // Reset all non-persistent Redirect configurations
            _diverter.ResetAll();

            if (output != null)
            {
                // Retarget ITestOutputHelper proxy calls to the current test output
                _diverter.Redirect<ITestOutputHelper>().Retarget(output);
            }

            return _diverter;
        }

        public IFooClient CreateFooClient()
        {
            return RestService.For<IFooClient>(CreateHttpClient());
        }
        
        private HttpClient CreateHttpClient()
        {
            var client = _webApplicationFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
