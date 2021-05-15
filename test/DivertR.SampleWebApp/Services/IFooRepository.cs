using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services
{
    public interface IFooRepository
    {
        Task<Foo> GetFoo(Guid id);
        Task<bool> TryInsertFoo(Foo foo);
    }
}