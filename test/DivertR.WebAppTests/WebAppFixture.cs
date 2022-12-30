using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DivertR.DependencyInjection;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Refit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests
{
    public class WebAppFixture
    {
        // Create a DivertR instance and register the DI services we want to be able to redirect
        private readonly IDiverter _diverter = new Diverter()
            .Register<ILoggerFactory>()
            .Register<IFooRepository>();

        private readonly WebApplicationFactory<Program> _webApplicationFactory;
        
        public WebAppFixture()
        {
            // Configure a persistent ViaRedirect on the ILoggerFactory to be able to redirect ILogger calls to Xunit output
            _diverter
                .Redirect<ILoggerFactory>()
                .To(x => x.CreateLogger(Is<string>.Any))
                .ViaRedirect(opt => opt.Persist()); // Persist option here means the Via is not removed when DivertR is reset
            
            _webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
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
                InitLogging(output);
            }

            return _diverter;
        }
        
        /// <summary>
        /// Retarget ILogger calls to the Xunit test output helper
        /// </summary>
        /// <param name="output">Xunit test output helper</param>
        private void InitLogging(ITestOutputHelper output)
        {
            var logger = output.BuildLogger(LogLevel.Information);
            
            _diverter
                .Redirect<ILogger>()
                .Retarget(logger);
        }

        private HttpClient CreateHttpClient()
        {
            var client = _webApplicationFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
        
        public IFooClient CreateFooClient()
        {
            return RestService.For<IFooClient>(CreateHttpClient());
        }
    }
}
