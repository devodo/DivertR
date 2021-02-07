using System;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;
using Refit;

namespace Divertr.WebAppTests
{
    public interface IFooClient
    {
        [Get("/foo/{id}")]
        Task<ApiResponse<Foo>> GetFoo(Guid id);
        
        [Post("/foo")]
        Task<ApiResponse<Foo>> InsertFoo(CreateFooRequest request);
    }
}