using DivertR.WebApp.Model;

namespace DivertR.WebApp.Services
{
    public interface IFooRepository
    {
        Task<Foo?> GetFooAsync(Guid id);
        Task InsertFooAsync(Foo foo);
    }
}