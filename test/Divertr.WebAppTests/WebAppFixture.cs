using System.Net.Http;
using System.Net.Http.Headers;
using Divertr.SampleWebApp;
using Divertr.SampleWebApp.Services;
using Divertr.ServiceCollection.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Refit;

namespace Divertr.WebAppTests
{
    public class WebAppFixture
    {
        public IDiverterSet DiverterSet { get; } = new DiverterSet();
        
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.DivertRange<IFooRepository, IFooRepository>(DiverterSet);
                });
            });
        }

        public HttpClient CreateHttpClient()
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