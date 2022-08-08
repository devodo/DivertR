using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services
{
    public interface IFooRepository
    {
        Task<Foo?> GetFooAsync(Guid id);
        Task<bool> TryInsertFooAsync(Foo foo);
    }
}