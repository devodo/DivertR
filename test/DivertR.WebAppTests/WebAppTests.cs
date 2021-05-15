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
                .Redirect(x => x.GetFoo(foo.Id))
                .To(Task.FromResult(foo));
            
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
            
            var callsRecord = _fooRepositoryVia.Record();
            _fooRepositoryVia
                .Redirect(x => x.GetFoo(Is<Guid>.Any))
                .To(Task.FromResult<Foo>(null));
            
            // ACT
            var response = await _fooClient.GetFoo(foo.Id);
            
            // ASSERT
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.ShouldBeNull();
            callsRecord.Count.ShouldBe(1);
            callsRecord.When(x => x.GetFoo(foo.Id)).Count.ShouldBe(1);
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
            var call = fooRepoCalls.When(x => x.TryInsertFoo(Is<Foo>.Match(f => f.Name == createFooRequest.Name))).Single();
            var insertedFoo = (Foo) call.CallInfo.Arguments[0];
            
            response.Headers.Location!.PathAndQuery.ShouldBe($"/Foo/{insertedFoo.Id}");
            insertedFoo.Name.ShouldBe(createFooRequest.Name);
            (await call.Returned!.Value).ShouldBe(true);
            response.Content.ShouldBeEquivalentTo(insertedFoo);
        }
    }
}
