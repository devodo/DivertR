using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DivertR.Record;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Services;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests
{
    public class WebAppTests : IClassFixture<WebAppFixture>
    {
        private readonly IVia<IFooRepository> _fooRepositoryVia;
        private readonly IFooClient _fooClient;

        public WebAppTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            var diverter = webAppFixture.InitDiverter(output);
            _fooRepositoryVia = diverter.Via<IFooRepository>();
            _fooClient = webAppFixture.CreateFooClient();
        }

        [Fact]
        public async Task GivenFooExists_WhenGetFoo_ThenReturnFooContent_WithOk200()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            _fooRepositoryVia
                .To(x => x.GetFoo(foo.Id))
                .Redirect(Task.FromResult(foo));
            
            // ACT
            var response = await _fooClient.GetFoo(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldBeEquivalentTo(foo);
        }
        
        [Fact]
        public async Task GivenFooDoesNotExist_WhenGetFoo_ThenReturn404NotFound()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            var fooRepoCalls = _fooRepositoryVia.Record(options => options.OrderWeight(int.MaxValue));
            _fooRepositoryVia
                .To(x => x.GetFoo(Is<Guid>.Any))
                .Redirect(Task.FromResult<Foo>(null));
            
            // ACT
            var response = await _fooClient.GetFoo(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();
            fooRepoCalls.Count.ShouldBe(1);
            fooRepoCalls.To(x => x.GetFoo(foo.Id)).Count().ShouldBe(1);
        }

        [Fact]
        public async Task WhenInsertFoo_ThenReturn201Created_WithGetLocation()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo insertedFoo = null;
            bool? insertResult = null;

            _fooRepositoryVia
                .Strict()
                .To(x => x.TryInsertFoo(Is<Foo>.Any))
                .Redirect<(Foo foo, __)>(async call =>
                {
                    insertedFoo = call.Args.foo;
                    insertResult = await call.Relay.Next.TryInsertFoo(call.Args.foo);
                    return insertResult.Value;
                });

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            insertResult.ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
        
        [Fact]
        public async Task RecordAlt_WhenInsertFoo_ThenReturn201Created_WithGetLocation()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var recordedCalls = _fooRepositoryVia.Record();

            // ACT  
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            recordedCalls.Count.ShouldBe(1);
            recordedCalls
                .To(x => x.TryInsertFoo(Is<Foo>.Match(f => f.Name == createFooRequest.Name)))
                .WithArgs<(Foo foo, __)>()
                .ForEach(call =>
                {
                    response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{call.Args.foo.Id}");
                    call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                    response.Content.ShouldBeEquivalentTo(call.Args.foo);
                    
                    call.Returned!.Value.Result.ShouldBe(true);
                }).Count().ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenFooRepositoryThrowsException_WhenInsertFoo_ThenReturns500InternalServerError()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            var testException = new Exception("test");

            var recordedCalls = _fooRepositoryVia
                .To(x => x.TryInsertFoo(Is<Foo>.Any))
                .Redirect(() => throw testException)
                .Record();

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            
            recordedCalls
                .ForEach(call => call.Returned!.Exception.ShouldBeSameAs(testException))
                .Count().ShouldBe(1);
        }
    }
}
