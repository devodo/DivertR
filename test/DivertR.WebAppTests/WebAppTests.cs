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
                .To(x => x.GetFooAsync(foo.Id))
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
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Redirect(Task.FromResult<Foo>(null));
            
            // ACT
            var response = await _fooClient.GetFoo(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();
            fooRepoCalls.Count.ShouldBe(1);
            fooRepoCalls.To(x => x.GetFooAsync(foo.Id)).Count().ShouldBe(1);
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
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Redirect<(Foo foo, __)>(async (call, args) =>
                {
                    insertedFoo = call.Args.foo;
                    insertResult = await call.Next.TryInsertFooAsync(args.foo);
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
        public async Task Record_WhenInsertFoo_ThenReturn201Created_WithGetLocation()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var insertCalls = _fooRepositoryVia
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Record<(Foo foo, __)>();

            // ACT  
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            (await insertCalls.ScanAsync(async call =>
            {
                response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{call.Args.foo.Id}");
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                response.Content.ShouldBeEquivalentTo(call.Args.foo);

                (await call.Returned!.Value).ShouldBe(true);
            })).ShouldBe(1);
        }
        
        [Fact]
        public async Task Spy_WhenInsertFoo_ThenReturn201Created_WithGetLocation()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var insertCalls = _fooRepositoryVia
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .WithArgs<(Foo foo, __)>()
                .Spy(async call => new
                {
                    Foo = call.Args.foo,
                    Result = await call.Returned!.Value
                });

            // ACT  
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            (await insertCalls.ScanAsync(call =>
            {
                response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{call.Foo.Id}");
                response.Content.ShouldBeEquivalentTo(call.Foo);
                call.Foo.Name.ShouldBe(createFooRequest.Name);
                call.Result.ShouldBe(true);
            })).ShouldBe(1);
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
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Redirect(() => throw testException)
                .Record();

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            recordedCalls
                .Scan(call => call.Returned!.Exception.ShouldBeSameAs(testException))
                .ShouldBe(1);
        }
    }
}
