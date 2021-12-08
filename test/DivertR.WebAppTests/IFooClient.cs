using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using Refit;

namespace DivertR.WebAppTests
{
    public interface IFooClient
    {
        [Get("/foo/{id}")]
        Task<ApiResponse<Foo>> GetFooAsync(Guid id);
        
        [Post("/foo")]
        Task<ApiResponse<Foo>> CreateFooAsync(CreateFooRequest request);
    }
}