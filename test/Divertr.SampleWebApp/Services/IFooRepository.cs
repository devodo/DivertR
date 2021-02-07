using System;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;

namespace Divertr.SampleWebApp.Services
{
    public interface IFooRepository
    {
        Task<Foo> GetFoo(Guid id);
        Task<bool> TryInsertFoo(Foo foo);
    }
}