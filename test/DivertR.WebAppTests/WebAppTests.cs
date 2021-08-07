using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DivertR.Redirects;
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
        public async Task GivenFooExists_ShouldReturnFooContent_WithOk200()
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
        public async Task GivenFooDoesNotExist_ShouldReturn404NotFound()
        {
            // ARRANGE
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString()
            };
            
            var fooRepoCalls = _fooRepositoryVia.Record();
            _fooRepositoryVia
                .To(x => x.GetFoo(Is<Guid>.Any))
                .Redirect(Task.FromResult<Foo>(null));
            
            // ACT
            var response = await _fooClient.GetFoo(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();
            fooRepoCalls.Count.ShouldBe(1);
            fooRepoCalls.To(x => x.GetFoo(foo.Id)).Count.ShouldBe(1);
        }

        [Fact]
        public async Task CanInsertFoo()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };

            Foo insertedFoo = null;
            bool? insertResult = null;

            _fooRepositoryVia
                .To(x => x.TryInsertFoo(Is<Foo>.Any))
                .Redirect(async (Foo foo) =>
                {
                    insertedFoo = foo;
                    insertResult = await _fooRepositoryVia.Next.TryInsertFoo(foo);
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
        public async Task CanRecordInsertFooCalls()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var fooRepoCalls = _fooRepositoryVia.Record();

            // ACT  
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            fooRepoCalls.Count.ShouldBe(1);
            fooRepoCalls
                .To(x => x.TryInsertFoo(Is<Foo>.Match(f => f.Name == createFooRequest.Name)))
                .Visit<Foo>(async (foo, callReturn) =>
                {
                    response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{foo.Id}");
                    foo.Name.ShouldBe(createFooRequest.Name);
                    (await callReturn.Value).ShouldBe(true);
                })
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task CanRecordFooRepositoryExceptionCalls()
        {
            // ARRANGE
            var createFooRequest = new CreateFooRequest
            {
                Name = Guid.NewGuid().ToString()
            };
            
            var fooRepoCalls = _fooRepositoryVia.Record();
            _fooRepositoryVia
                .To(x => x.TryInsertFoo(Is<Foo>.Any))
                .Redirect(() => throw new Exception("test"));

            // ACT
            var response = await _fooClient.InsertFoo(createFooRequest);

            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            
            fooRepoCalls.Count.ShouldBe(1);
            fooRepoCalls
                .To(x => x.TryInsertFoo(Is<Foo>.Match(f => f.Name == createFooRequest.Name)))
                .Visit<Foo>(async (_, callReturn) =>
                {
                    callReturn.Exception.ShouldBeOfType<Exception>();
                })
                .Count.ShouldBe(1);
        }
    }
}
