using DivertR.WebApp.Model;

namespace DivertR.WebApp.Services;

public class FooService : IFooService
{
    private readonly IFooRepository _fooRepository;

    public FooService(IFooRepository fooRepository)
    {
        _fooRepository = fooRepository;
    }
    
    public Task<Foo?> GetFooAsync(Guid id)
    {
        return _fooRepository.GetFooAsync(id);
    }

    public async Task<Foo> CreateFooAsync(Guid id, string name)
    {
        var foo = new Foo
        {
            Id = id,
            Name = name
        };
        
        await _fooRepository.InsertFooAsync(foo);

        return foo;
    }
}