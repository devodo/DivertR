using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Rest;
using Refit;

namespace DivertR.WebAppTests.TestHarness
{
    public interface IFooClient
    {
        [Get("/foo/{id}")]
        Task<ApiResponse<Foo>> GetFooAsync(Guid id);
        
        [Post("/foo")]
        Task<ApiResponse<Foo>> CreateFooAsync(CreateFooRequest request);
    }
}