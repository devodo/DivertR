using System;
using System.Net;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Services;
using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests
{
    public class WebAppTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverter _diverter;
        private readonly IVia<IFooRepository> _fooRepositoryVia;
        private readonly IFooClient _fooClient;
        
        private readonly Mock<IFooRepository> _fooRepositoryMock = new();
        private readonly IFooRepository _fooRepositoryFake;
        
        public WebAppTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverter = webAppFixture.InitDiverter(output);
            _fooRepositoryVia = _diverter.Via<IFooRepository>();

            _fooRepositoryFake = A.Fake<IFooRepository>(o =>
                o.Wrapping(_diverter.Via<IFooRepository>().Relay.Next));
            _diverter.Via<IFooRepository>().RedirectTo(_fooRepositoryFake);
            
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
                .ReturnsLazily(() => Task.FromResult(foo));

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
                    var relay = _diverter.Via<IFooRepository>().Relay.Next;
                    var result = await relay.TryInsertFoo(foo);
                    return result;
                });

            _diverter.Via<IFooRepository>().Reset().RedirectTo(_fooRepositoryMock.Object);
            
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

        [Fact]
        public async Task CanInsertFooRedirect()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo insertedFoo = null;
            bool? insertResult = null;

            _fooRepositoryVia
                .Redirect(x => x.TryInsertFoo(Is<Foo>.Any))
                .To(async (Foo foo) =>
                {
                    insertedFoo = foo;
                    insertResult = await _fooRepositoryVia.Next.TryInsertFoo(foo);
                    return insertResult.Value;
                });

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            insertResult.ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
        
        [Fact]
        public async Task CanRecordInsertFooRedirectCalls()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var callsRecord = _fooRepositoryVia.RecordCalls();

            _fooRepositoryVia
                .Redirect(x => x.TryInsertFoo(Is<Foo>.Any))
                .To(() => (Task<bool>) _fooRepositoryVia.Relay.CallNext());

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var calls = callsRecord.Calls(x => x.TryInsertFoo(Is<Foo>.Match(f => f.Name == createFooRequest.Name)));
            calls.Count.ShouldBe(1);
            var insertResult = await (Task<bool>) calls[0].ReturnValue;
            var insertedFoo = (Foo) calls[0].CallInfo.Arguments[0];
            response.Headers.Location.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            insertResult.ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
    }
}