using System;
using System.Net;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests
{
    public class WebAppTests : IClassFixture<WebAppFixture>
    {
        private readonly IVia<IFooRepository> _fooRepositoryVia;
        private readonly IFooClient _fooClient;
        private readonly IServiceProvider _services;

        public WebAppTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            var diverter = webAppFixture.InitDiverter(output);
            _fooRepositoryVia = diverter.Via<IFooRepository>();
            _fooClient = webAppFixture.CreateFooClient();
            _services = webAppFixture.Services;
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
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.ShouldBeEquivalentTo(foo);
        }
        
        [Fact]
        public async Task GivenFooExists_WhenGetFoo_ThenReadsFromFooRepository()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            var fooRepository = _services.GetRequiredService<IFooRepository>();
            (await fooRepository.TryInsertFooAsync(foo)).ShouldBeTrue();

            (Guid fooId, Foo foo) fooRepoCall = default;

            _fooRepositoryVia
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Redirect<(Guid fooId, __)>(async (call, args) =>
                {
                    var getFoo = await call.Root.GetFooAsync(args.fooId);
                    fooRepoCall = (args.fooId, getFoo);

                    return getFoo;
                });
            
            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.Content.ShouldBeEquivalentTo(foo);
            fooRepoCall.foo.ShouldBeEquivalentTo(foo);
            fooRepoCall.fooId.ShouldBe(foo.Id);
        }
        
        [Fact]
        public async Task GivenFooDoesNotExist_WhenGetFoo_ThenReturn404NotFound()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid()
            };

            var getFooCalls = _fooRepositoryVia
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Redirect<(Guid fooId, __)>(Task.FromResult<Foo>(null))
                .Record();
            
            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();

            (await getFooCalls.Replay(async call =>
            {
                call.Args.fooId.ShouldBe(foo.Id);
                call.Returned?.IsException.ShouldBeFalse();
                (await call.Returned!.Value).ShouldBeNull();
            })).ShouldBe(1);
        }

        [Fact]
        public async Task GiveFooNotExists_WhenCreateFooRequest_ThenInsertsFooAndReturn201CreatedWithLocationHeader()
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
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            insertResult.ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
        
        [Fact]
        public async Task GiveFooNotExists_WhenCreateFooRequest_ThenInsertsFoo_RecordExample()
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
            await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            (await insertCalls.Replay(async call =>
            {
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                call.Returned?.IsValue.ShouldBeTrue();
                (await call.Returned!.Value).ShouldBe(true);
            })).ShouldBe(1);
        }
        
        [Fact]
        public async Task GiveFooNotExists_WhenCreateFooRequest_ThenInsertsFoo_RecordMapExample()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var insertCalls = _fooRepositoryVia
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Record<(Foo foo, __)>()
                .Map((call, args) => new
                {
                    Foo = args.foo,
                    Result = call.Returned
                });

            // ACT  
            await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            (await insertCalls.Replay(async call =>
            {
                call.Foo.Name.ShouldBe(createFooRequest.Name);
                call.Result?.IsValue.ShouldBeTrue();
                (await call.Result!.Value).ShouldBe(true);
            })).ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenFooRepositoryThrowsException_WhenCreateFooRequest_ThenReturns500InternalServerError()
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
                .Record<(Foo foo, __)>();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            recordedCalls
                .Replay((call, args) =>
                {
                    args.foo.Name.ShouldBe(createFooRequest.Name);
                    call.Returned!.Exception.ShouldBeSameAs(testException);
                })
                .ShouldBe(1);
        }
    }
}
