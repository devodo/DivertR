using System;
using System.Linq;
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
        public async Task GivenFooExistsInRepo_WhenGetFoo_ThenReturnsFoo_WithOk200()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = "Foo123"
            };

            _diverter
                .Redirect<IFooRepository>()
                .To(x => x.GetFooAsync(foo.Id))
                .Via(Task.FromResult<Foo?>(foo));

            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Content.Id.ShouldBe(foo.Id);
            response.Content.Name.ShouldBe(foo.Name);
        }
        
        [Fact]
        public async Task GivenAllFoosExistInRepo_WhenConcurrentGetFoo_ThenReturnsFoos_WithOk200()
        {
            // ARRANGE
            var fooIds = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToArray();
            
            _diverter
                .Redirect<IFooRepository>()
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Via<(Guid fooId, __)>(call => Task.FromResult<Foo?>(new Foo
                {
                    Id = call.Args.fooId,
                    Name = $"{call.Args.fooId}"
                }));
            
            // ACT
            var responses = await Task.WhenAll(fooIds.Select(fooId => _fooClient.GetFooAsync(fooId)));
            
            // ASSERT
            responses.ShouldAllBe(response => response.StatusCode == HttpStatusCode.OK);
            responses.Select(response => response.Content.Id).ShouldBe(fooIds);
            responses.Select(response => response.Content.Name).ShouldBe(fooIds.Select(fooId => $"{fooId}"));
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
            
            // Record all repo calls
            var fooRepoCalls = _diverter
                .Redirect<IFooRepository>()
                .Record();

            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.Content.ShouldBeEquivalentTo(foo);
            
            // Verify repo get method called correctly
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
                .Redirect<IFooRepository>()
                .To(x => x.GetFooAsync(fooId))
                .Via<(Guid fooId, __)>(Task.FromResult<Foo?>(null))
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
        public async Task GiveFooDoesNotExist_WhenCreateFoo_ThenInsertsFooAndReturn201CreatedWithLocationHeader()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo? insertedFoo = null;
            bool? insertResult = null;

            _diverter
                .Redirect<IFooRepository>()
                .Strict()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Via<(Foo foo, __)>(async (call, args) =>
                {
                    insertedFoo = args.foo;
                    insertResult = await call.CallNext();
                    
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
                .Redirect<IFooIdGenerator>()
                .To(x => x.Create())
                .Via(fooId)
                .Record();

            var insertCalls = _diverter
                .Redirect<IFooRepository>()
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
                .Redirect<IFooRepository>()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Via<(Foo foo, __)>(() => throw testException)
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
                .Redirect<IFooRepository>()
                .To(x => x.TryInsertFooAsync(Is<Foo>.Any))
                .Via<(Foo foo, __)>(Task.FromResult(false))
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
