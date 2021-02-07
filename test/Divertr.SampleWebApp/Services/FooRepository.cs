using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;

namespace Divertr.SampleWebApp.Services
{
    public class FooRepository : IFooRepository
    {
        private static readonly ConcurrentDictionary<Guid, Foo> FooStore = new();
        
        public Task<Foo> GetFoo(Guid id)
        {
            return FooStore.TryGetValue(id, out var foo) 
                ? Task.FromResult(foo)
                : Task.FromResult((Foo)null);
        }

        public Task<bool> TryInsertFoo(Foo foo)
        {
            return Task.FromResult(FooStore.TryAdd(foo.Id, foo));
        }
    }
}