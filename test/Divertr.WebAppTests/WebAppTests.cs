using System;
using System.Net;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;
using Divertr.SampleWebApp.Services;
using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;

namespace Divertr.WebAppTests
{
    public class WebAppTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverterSet _divertr;
        private readonly IFooClient _fooClient;
        
        private readonly Mock<IFooRepository> _fooRepositoryMock = new();
        private readonly IFooRepository _originalFooRepository;
        private readonly IFooRepository _fooRepositoryFake;
        
        public WebAppTests(WebAppFixture webAppFixture)
        {
            _divertr = webAppFixture.DiverterSet;
            _divertr.ResetAll();
            _originalFooRepository = _divertr.Get<IFooRepository>().CallCtx.Original;
            _fooRepositoryFake  = A.Fake<IFooRepository>(o => o.Wrapping(_originalFooRepository));
            _divertr.Get<IFooRepository>().Redirect(_fooRepositoryFake);
            
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
                    var result = await _originalFooRepository.TryInsertFoo(foo);
                    return result;
                });

            _divertr.Get<IFooRepository>().Redirect(_fooRepositoryMock.Object);
            
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
                .ReturnsLazily((Foo foo) =>
                {
                    insertedFoo = foo;
                    return _originalFooRepository.TryInsertFoo(foo);
                });
            
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