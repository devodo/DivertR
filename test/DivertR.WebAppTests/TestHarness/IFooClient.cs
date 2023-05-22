using System;
using System.Threading.Tasks;
using DivertR.WebApp.Rest;
using Refit;

namespace DivertR.WebAppTests.TestHarness
{
    public interface IFooClient
    {
        [Get("/foo/{id}")]
        Task<ApiResponse<FooResponse>> GetFooAsync(Guid id);
        
        [Post("/foo")]
        Task<ApiResponse<FooResponse>> CreateFooAsync(CreateFooRequest request);
        
        [Post("/bar/{name}")]
        Task<ApiResponse<BarResponse>> CreateBarAsync(string name);
    }
}