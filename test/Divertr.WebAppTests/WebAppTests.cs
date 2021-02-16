using System;
using System.Net;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;
using Divertr.SampleWebApp.Services;
using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Divertr.WebAppTests
{
    public class WebAppTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverterCollection _diverters;
        private readonly IFooClient _fooClient;
        
        private readonly Mock<IFooRepository> _fooRepositoryMock = new();
        private readonly IFooRepository _fooRepositoryRoot;
        private readonly IFooRepository _fooRepositoryFake;
        
        public WebAppTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverters = webAppFixture.InitDiverter(output);
            _fooRepositoryRoot = _diverters.Of<IFooRepository>().CallCtx.Root;
            _fooRepositoryFake  = A.Fake<IFooRepository>(o => o.Wrapping(_fooRepositoryRoot));
            _diverters.Of<IFooRepository>().SendTo(_fooRepositoryFake);
            
            _fooClient = webAppFixture.CreateFooClient();
        }

        [Fact]
        public async Task CanGetFoo()
        {
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            A.CallTo(() => _fooRepositoryFake.GetFoo(foo.Id))
                .ReturnsLazily( () => Task.FromResult(foo));

            var response = await _fooClient.GetFoo(foo.Id);
            
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldBeEquivalentTo(foo);
        }

        [Fact]
        public async Task CanInsertFooMock()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo insertedFoo = null;
            _fooRepositoryMock
                .Setup(x => x.TryInsertFoo(It.IsAny<Foo>()))
                .Returns(async (Foo foo) =>
                {
                    insertedFoo = foo;
                    var result = await _fooRepositoryRoot.TryInsertFoo(foo);
                    return result;
                });

            _diverters.Of<IFooRepository>().SendTo(_fooRepositoryMock.Object);
            
            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
        
        [Fact]
        public async Task CanInsertFooFake()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo insertedFoo = null;
            A.CallTo(() => _fooRepositoryFake.TryInsertFoo(A<Foo>._))
                .Invokes((Foo foo) =>
                {
                    insertedFoo = foo;
                })
                .CallsWrappedMethod();

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
    }
}