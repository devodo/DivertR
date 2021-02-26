using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using Refit;

namespace DivertR.WebAppTests
{
    public interface IFooClient
    {
        [Get("/foo/{id}")]
        Task<ApiResponse<Foo>> GetFoo(Guid id);
        
        [Post("/foo")]
        Task<ApiResponse<Foo>> InsertFoo(CreateFooRequest request);
    }
}