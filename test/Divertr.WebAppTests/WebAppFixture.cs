using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Divergic.Logging.Xunit;
using Divertr.Extensions.DependencyInjection;
using Divertr.SampleWebApp;
using Divertr.SampleWebApp.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using Xunit.Abstractions;

namespace Divertr.WebAppTests
{
    public class WebAppFixture
    {
        public IDiverterCollection Diverters { get; } = new DiverterCollection();
        public List<Type> TypesRegistered { get; private set; }
        
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Divert(Diverters, diverterBuilder =>
                    {
                        diverterBuilder.IncludeFrom<IFooRepository>();
                        diverterBuilder.ExcludeFrom<IFooPublisher>(inclusive: false);
                        diverterBuilder.Include<ILoggerFactory>();
                        diverterBuilder.WithDiversionsRegisteredHandler(TypeRegisteredHandler);
                    });
                });
            });
        }

        public IDiverterCollection InitDiverter(ITestOutputHelper output = null)
        {
            Diverters.ResetAll();

            if (output != null)
            {
                InitLogging(output);
            }

            return Diverters;
        }

        public void InitLogging(ITestOutputHelper output)
        {
            var fakeLoggerFactory = A.Fake<ILoggerFactory>();
            A.CallTo(() => fakeLoggerFactory.CreateLogger(A<string>._))
                .ReturnsLazily((string name) =>
                {
                    if (name.StartsWith("Microsoft"))
                    {
                        return output.BuildLogger(LogLevel.Warning, name);
                    }
                    
                    return output.BuildLogger(name);
                });
            
            Diverters.Of<ILoggerFactory>().SendTo(fakeLoggerFactory);
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