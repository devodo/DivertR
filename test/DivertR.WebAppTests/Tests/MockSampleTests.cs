using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Rest;
using DivertR.SampleWebApp.Services;
using DivertR.WebAppTests.TestHarness;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.WebAppTests.Tests
{
    public class MockSampleTests : IClassFixture<WebAppFixture>
    {
        private readonly IDiverter _diverter;
        private readonly IFooClient _fooClient;

        public MockSampleTests(WebAppFixture webAppFixture, ITestOutputHelper output)
        {
            _diverter = webAppFixture.InitDiverter(output);
            _fooClient = webAppFixture.CreateFooClient();
        }

        [Fact]
        public async Task GivenFooExistsInRepository_WhenGetFoo_Then200OkResponseWithFooContent()
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
            response.Content!.Id.ShouldBe(foo.Id);
            response.Content.Name.ShouldBe(foo.Name);
        }
        
        [Fact]
        public async Task GivenAnyFooExistInRepository_WhenConcurrentGetFoo_Then200OkResponseWithFooContent()
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
            responses.Select(response => response.Content!.Id).ShouldBe(fooIds);
            responses.Select(response => response.Content!.Name).ShouldBe(fooIds.Select(fooId => $"{fooId}"));
        }


        [Fact]
        public async Task GiveFooDoesNotExist_WhenCreateFoo_Then201CreatedResponseWithLocationHeaderAndInsertsFoo()
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
                    insertedFoo = call.Args.foo;
                    
                    return call.CallNext();
                });

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{insertedFoo!.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            response.Content.ShouldNotBeNull();
            response.Content.Id.ShouldBe(insertedFoo.Id);
            response.Content.Name.ShouldBe(insertedFoo.Name);
        }
        
        [Fact]
        public async Task GivenFooRepositoryInsertFails_WhenCreateFooRequest_Then500InternalServerErrorResponse()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };

            var testException = new Exception("test");

            _diverter
                .Redirect<IFooRepository>()
                .To(x => x.InsertFooAsync(Is<Foo>.Any))
                .Via(() => throw testException);

            // ACT
            var response = await _fooClient.CreateFooAsync(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
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
    }
}
