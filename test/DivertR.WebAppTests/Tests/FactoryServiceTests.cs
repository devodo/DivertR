using System;
using System.Net;
using System.Threading.Tasks;
using DivertR.WebApp.Model;
using DivertR.WebApp.Services;
using DivertR.WebAppTests.TestHarness;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests.Tests
{
    public class FactoryServiceTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverter _diverter;
        private readonly IFooClient _fooClient;

        public FactoryServiceTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverter = webAppFixture.InitDiverter(output);
            _fooClient = webAppFixture.CreateFooClient();
        }
        
        [Fact]
        public async Task GivenMockedBarService_WhenCreateBar_Then200OkResponseWithCreatedBarContent()
        {
            // ARRANGE
            var bar = new Bar
            {
                Id = Guid.NewGuid(), Label = Guid.NewGuid().ToString("N"), CreatedDate = DateTime.UtcNow
            };
            
            // IBarService is an "inner" service created by a factory (and not the DI container)
            _diverter
                .Redirect<IBarService>()
                .To(x => x.CreateBarAsync(bar.Label))
                .Via(() => Task.FromResult(bar));

            // ACT
            var response = await _fooClient.CreateBarAsync(bar.Label);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldNotBeNull();
            response.Content.Id.ShouldBe(bar.Id);
            response.Content.Label.ShouldBe(bar.Label);
            response.Content.CreatedDate.ShouldBe(bar.CreatedDate);
        }
    }
}
