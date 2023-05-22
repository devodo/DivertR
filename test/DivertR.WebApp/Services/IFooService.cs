using DivertR.WebApp.Model;

namespace DivertR.WebApp.Services
{
    public interface IFooService
    {
        Task<Foo?> GetFooAsync(Guid id);
        Task<Foo> CreateFooAsync(Guid id, string name);
    }
}