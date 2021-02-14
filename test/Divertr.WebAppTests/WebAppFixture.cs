﻿using System;
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
                        diverterBuilder.ExcludeRange<IFooPublisher>(startInclusive: false);
                        diverterBuilder.Include<ILoggerFactory>();
                        diverterBuilder.WithTypesRegisteredHandler(TypeRegisteredHandler);
                    });
                });
            });
        }

        public IDiverter InitDiverter(ITestOutputHelper output = null)
        {
            Diverter.ResetAll();

            if (output != null)
            {
                InitLogging(output);
            }

            return Diverter;
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
            
            Diverter.Of<ILoggerFactory>().Redirect(fakeLoggerFactory);
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