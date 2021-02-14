using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Divertr.Extensions.DependencyInjection;
using Divertr.SampleWebApp;
using Divertr.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Refit;

namespace Divertr.WebAppTests
{
    public class WebAppFixture
    {
        public IDiverter Diverter { get; } = new Diverter();
        public List<Type> TypesRegistered { get; private set; }
        
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Divert(Diverter, diverterBuilder =>
                    {
                        diverterBuilder.IncludeRange<IFooRepository>();
                        diverterBuilder.ExcludeRange<IFooRepository>(startInclusive: false);
                        diverterBuilder.WithTypesRegisteredHandler(TypeRegisteredHandler);
                    });
                });
            });
        }

        public IDiverter InitDiverter()
        {
            return Diverter.ResetAll();
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

        private void TypeRegisteredHandler(object sender, IEnumerable<Type> typesRegistered)
        {
            TypesRegistered = typesRegistered.ToList();
        }
    }
}