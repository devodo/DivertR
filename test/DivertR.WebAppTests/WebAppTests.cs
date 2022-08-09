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
        private readonly IDiverter _diverter;
        private readonly IFooClient _fooClient;
        private readonly IServiceProvider _services;

        public WebAppTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverter = webAppFixture.InitDiverter(output);
            _fooClient = webAppFixture.CreateFooClient();
            _services = webAppFixture.Services;
        }

        [Fact]
        public async Task GivenFooExistsInRepo_WhenGetFoo_ThenReturnFooContent_WithOk200()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = "Foo123"
            };

            _diverter
                .Via<IFooRepository>()
                .To(x => x.GetFooAsync(foo.Id))
                .Redirect(Task.FromResult<Foo?>(foo));

            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.Id.ShouldBe(foo.Id);
            response.Content.Name.ShouldBe(foo.Name);
        }
        
        [Fact]
        public async Task GivenAnyFooExistsInRepo_WhenGetFoo_ThenReturnFooContent_WithOk200()
        {
            // ARRANGE
            var fooId = Guid.NewGuid();
            
            _diverter
                .Via<IFooRepository>()
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Redirect<(Guid fooId, __)>(call => Task.FromResult<Foo?>(new Foo
                {
                    Id = call.Args.fooId,
                    Name = $"{call.Args.fooId}"
                }));
            
            // ACT
            var response = await _fooClient.GetFooAsync(fooId);
            
            // ASSERT
            response.Content.Id.ShouldBe(fooId);
            response.Content.Name.ShouldBe($"{fooId}");
        }
        
        [Fact]
        public async Task GivenFooInserted_WhenGetFoo_ThenReadsFromFooRepository()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            // Insert foo directly into the repository
            var fooRepository = _services.GetRequiredService<IFooRepository>();
            (await fooRepository.TryInsertFooAsync(foo)).ShouldBeTrue();

            var fooRepoCalls = _diverter
                .Via<IFooRepository>()
                .Record();

            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.Content.ShouldBeEquivalentTo(foo);
            
            // Verify repo read call
            (await fooRepoCalls
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Verify<(Guid fooId, __)>(async call =>
                {
                    call.Args.fooId.ShouldBe(foo.Id);
                    (await call.Returned!.Value!).ShouldBeEquivalentTo(foo);
                })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenFooDoesNotExist_WhenGetFoo_ThenReturn404NotFound()
        {
            // ARRANGE
            var fooId = Guid.NewGuid();

            var getFooCalls = _diverter
                .Via<IFooRepository>()
                .To(x => x.GetFooAsync(fooId))
                .Redirect<(Guid fooId, __)>(Task.FromResult<Foo?>(null))
                .Record();
            
            // ACT
            var response = await _fooClient.GetFooAsync(fooId);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();

            (await getFooCalls.Verify(async (call, args) =>
            {
                args.fooId.ShouldBe(fooId);
                (await call.Returned!.Value!).ShouldBeNull();
            })).Count.ShouldBe(1);
        }

        [Fact]
        public async Task GiveFooNotExists_WhenCreateFooRequest_ThenInsertsFooAndReturn201CreatedWithLocationHeader()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo? insertedFoo = null;
            bool? insertResult = null;

            _diverter
                .Via<IFooRepository>()
                .Strict()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Redirect<(Foo foo, __)>(async (call, args) =>
                {
                    insertedFoo = args.foo;
                    insertResult = await call.Next.TryInsertFooAsync(args.foo);
                    return insertResult.Value;
                });

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{insertedFoo!.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            insertResult.ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
        
        [Fact]
        public async Task GiveFooNotExists_WhenCreateFooRequest_ThenInsertsFooIntoRepository()
        {
            // ARRANGE
            var fooId = Guid.NewGuid();
            
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            var createFooIdCalls = _diverter
                .Via<IFooIdGenerator>()
                .To(x => x.Create())
                .Redirect(fooId)
                .Record();

            var insertCalls = _diverter
                .Via<IFooRepository>()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Record<(Foo foo, __)>();

            // ACT  
            await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            createFooIdCalls.Count.ShouldBe(1);
            
            (await insertCalls.Verify(async (call, args) =>
            {
                args.foo.Id.ShouldBe(fooId);
                args.foo.Name.ShouldBe(createFooRequest.Name);
                (await call.Returned!.Value!).ShouldBe(true);
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenFooRepositoryInsertFails_WhenCreateFooRequest_ThenReturns500InternalServerError()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            var testException = new Exception("test");

            var recordedCalls = _diverter
                .Via<IFooRepository>()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Redirect<(Foo foo, __)>(() => throw testException)
                .Record();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            recordedCalls.Verify((call, args) =>
            {
                args.foo.Name.ShouldBe(createFooRequest.Name);
                call.Returned!.Exception.ShouldBeSameAs(testException);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GiveFooExists_WhenCreateFooRequest_ThenRepoInsertReturnsFalse()
        {
            // ARRANGE
            
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            var insertCalls = _diverter
                .Via<IFooRepository>()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Redirect<(Foo foo, __)>(Task.FromResult(false))
                .Record();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
            
            (await insertCalls.Verify(async call =>
            {
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                (await call.Returned!.Value!).ShouldBe(false);
            })).Count.ShouldBe(1);
        }
    }
}
