using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using Microsoft.Extensions.Logging;

namespace DivertR.SampleWebApp.Services
{
    public class FooRepository : IFooRepository
    {
        private readonly ILogger<FooRepository> _logger;

        public FooRepository(ILogger<FooRepository> logger)
        {
            _logger = logger;
        }
        
        private static readonly ConcurrentDictionary<Guid, Foo> FooStore = new();
        
        public Task<Foo?> GetFooAsync(Guid id)
        {
            return FooStore.TryGetValue(id, out var foo) 
                ? Task.FromResult((Foo?) foo)
                : Task.FromResult<Foo?>(null);
        }

        public Task<bool> TryInsertFooAsync(Foo foo)
        {
            _logger.LogInformation("Inserting foo {FooId}", foo.Id);
            var inserted = FooStore.TryAdd(foo.Id, foo);
            
            return Task.FromResult(inserted);
        }
    }
}