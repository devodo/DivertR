using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DivertR.DependencyInjection;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Refit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests.TestHarness
{
    public class WebAppFixture
    {
        // Create a DivertR instance and register the DI services we want to be able to redirect
        private readonly IDiverter _diverter = new Diverter()
            .Register<IFooRepository>()
            .Register<IBarServiceFactory>(x => x
                .ThenRegister<IBarService>()) // Nested registrations allow redirecting inner services created outside DI e.g. by factories
            .Register<IFooService>();

        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        
        public WebAppFixture()
        {
            // Create an xUnit ITestOutputHelper proxy mock
            var outputHelperMock = _diverter.RedirectSet.GetOrCreate<ITestOutputHelper>().Proxy();

            _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging =>
                {
                    // Add an xUnit logging provider that writes to the mock ITestOutputHelper
                    logging.AddXunit(outputHelperMock);
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
            // Reset all non-persistent Redirect Vias
            _diverter.ResetAll();

            if (output != null)
            {
                // Retarget the ITestOutputHelper proxy mock to the current test output
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
