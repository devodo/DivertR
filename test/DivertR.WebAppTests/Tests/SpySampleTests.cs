using System;
using System.Net;
using System.Threading.Tasks;
using DivertR.WebApp.Model;
using DivertR.WebApp.Rest;
using DivertR.WebApp.Services;
using DivertR.WebAppTests.TestHarness;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests.Tests
{
    public class SpySampleTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverter _diverter;
        private readonly IFooClient _fooClient;
        private readonly IServiceProvider _services;

        public SpySampleTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverter = webAppFixture.InitDiverter(output);
            _fooClient = webAppFixture.CreateFooClient();
            _services = webAppFixture.Services;
        }
        
        [Fact]
        public async Task GiveFooDoesNotExist_WhenCreateFoo_ThenRepositoryInsertsFoo()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            Foo? insertedFoo = null;

            _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Via<(Foo foo, __)>(call =>
                {
                    // Capture foo argument passed to repository call
                    insertedFoo = call.Args.foo;
                    
                    // Continue repository call
                    return call.CallNext();
                });

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            insertedFoo.ShouldNotBeNull();
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            response.Content.ShouldNotBeNull();
            response.Content.Id.ShouldBe(insertedFoo.Id);
            response.Content.Name.ShouldBe(insertedFoo.Name);
        }
        
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public async Task GiveFooDoesNotExist_WhenCreateFoo_ThenInsertsFooIntoRepository()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            var insertCalls = _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Record<(Foo foo, __)>();

            // ACT  
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            insertCalls.Verify(call =>
            {
                call.Args.foo.Id.ShouldBe(createFooRequest.Id.Value);
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                call.Return.ShouldBe(Task.CompletedTask);
                response.Content.ShouldNotBeNull();
                response.Content.Id.ShouldBe(call.Args.foo.Id);
                response.Content.Name.ShouldBe(call.Args.foo.Name);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GiveBadCreateFooRequest_WhenCreateFoo_ThenDoesNotCallFooService()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = null
            };

            var fooServiceCalls = _diverter
                .Redirect<IFooService>()
                .Record();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            fooServiceCalls.Count.ShouldBe(0);
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
            await fooRepository.InsertFooAsync(foo);
            
            // Record get repo calls
            var fooRepoCalls = _diverter
                .Redirect<IFooRepository>()
                .To(x => x.GetFooAsync(Is<Guid>.Any))
                .Record();

            // ACT
            var response = await _fooClient.GetFooAsync(foo.Id);
            
            // ASSERT
            response.Content.ShouldNotBeNull();
            response.Content.Id.ShouldBe(foo.Id);
            response.Content.Name.ShouldBe(foo.Name);
            
            // Verify repo get method called correctly
            (await fooRepoCalls
                .Verify<(Guid fooId, __)>(async call =>
                {
                    call.Args.fooId.ShouldBe(foo.Id);
                    (await call.Return!).ShouldBeEquivalentTo(foo);
                })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenFooDoesNotExist_WhenGetFoo_ThenRepositoryReturnsNull()
        {
            // ARRANGE
            var fooId = Guid.NewGuid();

            var getFooCalls = _diverter
                .Redirect<IFooRepository>()
                .To(x => x.GetFooAsync(fooId))
                .Record<(Guid fooId, __)>();
            
            // ACT
            var response = await _fooClient.GetFooAsync(fooId);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();

            (await getFooCalls.Verify(async call =>
            {
                call.Args.fooId.ShouldBe(fooId);
                (await call.Return!).ShouldBeNull();
            })).Count.ShouldBe(1);
        }

        [Fact]
        public async Task GiveRepositoryThrowsDuplicateFooException_WhenCreateFooRequest_Then409ConflictResponseWithExistingFooContent()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Via<(Foo foo, __)>(call =>
                {
                    var existingFoo = new Foo { Id = call.Args.foo.Id, Name = "existing" };

                    throw new DuplicateFooException(existingFoo, "test");
                });

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
            var responseContent = await response.Error!.GetContentAsAsync<FooResponse>();
            responseContent!.Id.ShouldBe(createFooRequest.Id.Value);
            responseContent.Name.ShouldBe("existing");
        }
        
        [Fact]
        public async Task GivenFooRepositoryInsertFails_WhenCreateFooRequest_ThenReturns500InternalServerError()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            var testException = new Exception("test");

            var recordedCalls = _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Via(() => throw testException)
                .Record<(Foo foo, __)>();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            recordedCalls.Verify(call =>
            {
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                call.Exception.ShouldBeSameAs(testException);
            }).Count.ShouldBe(1);
        }

        [Fact]
        public async Task GivenFooAlreadyExistsInRepository_WhenCreateFooRequest_Then409ConflictResponseWithFooContent()
        {
            // ARRANGE
            // Insert foo directly into the repository
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            var fooRepository = _services.GetRequiredService<IFooRepository>();
            await fooRepository.InsertFooAsync(foo);

            var createFooRequest = new CreateFooRequest
            {
                Id = foo.Id,
                Name = Guid.NewGuid().ToString()
            };

            var recordedCalls = _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Record<(Foo foo, __)>();

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
            recordedCalls.Verify(call =>
            {
                call.Args.foo.Name.ShouldBe(createFooRequest.Name);
                call.Args.foo.Id.ShouldBe(createFooRequest.Id!.Value);
                var duplicateException = call.Exception.ShouldBeOfType<DuplicateFooException>();
                duplicateException.Foo.Id.ShouldBe(call.Args.foo.Id);
            }).Count.ShouldBe(1);
        }
    }
}
