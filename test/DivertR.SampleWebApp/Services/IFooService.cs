using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services
{
    public interface IFooService
    {
        Task<Foo?> GetFooAsync(Guid id);
        Task<Foo> CreateFooAsync(Guid id, string name);
    }
}