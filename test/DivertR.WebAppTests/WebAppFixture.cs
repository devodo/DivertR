﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using DivertR.DependencyInjection;
using DivertR.SampleWebApp;
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
        private readonly IDiverter _diverter = new Diverter()
            .Register<ILoggerFactory>()
            .Register<IFooRepository>()
            .Register<IFooIdGenerator>();

        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        
        public WebAppFixture()
        {
            _webApplicationFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.Divert(_diverter);
                });
            });
        }

        public IServiceProvider Services => _webApplicationFactory.Services;

        public IDiverter InitDiverter(ITestOutputHelper? output = null)
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
            _diverter.Redirect<ILoggerFactory>()
                .To(x => x.CreateLogger(Is<string>.Any))
                .Via<(string name, __)>(call =>
                {
                    if (call.Args.name.StartsWith("Microsoft"))
                    {
                        return output.BuildLogger(LogLevel.Warning, call.Args.name);
                    }
                    
                    return output.BuildLogger(call.Args.name);
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
