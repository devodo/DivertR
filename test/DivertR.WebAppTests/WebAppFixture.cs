using System.Net.Http;
using System.Net.Http.Headers;
using DivertR.DependencyInjection;
using DivertR.SampleWebApp;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Refit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests
{
    public class WebAppFixture
    {
        private readonly IDiverter _diverter = new Diverter().Register(new[]
        {
            typeof(ILoggerFactory), typeof(IFooRepository), typeof(IFooPublisher)
        });

        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Divert(_diverter);
                });
            });
        }

        public IDiverter InitDiverter(ITestOutputHelper output = null)
        {
            _diverter.ResetAll();

            if (output != null)
            {
                InitLogging(output);
            }

            return _diverter;
        }

        private void InitLogging(ITestOutputHelper output)
        {
            _diverter.Via<ILoggerFactory>()
                .To(x => x.CreateLogger(Is<string>.Any))
                .Redirect((string name) =>
                {
                    if (name.StartsWith("Microsoft"))
                    {
                        return output.BuildLogger(LogLevel.Warning, name);
                    }
                    
                    return output.BuildLogger(name);
                });
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
