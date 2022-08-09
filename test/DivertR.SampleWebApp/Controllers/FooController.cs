using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DivertR.SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase
    {
        private readonly IFooRepository _fooRepository;
        private readonly IFooIdGenerator _fooIdGenerator;

        public FooController(IFooRepository fooRepository, IFooIdGenerator fooIdGenerator)
        {
            _fooRepository = fooRepository ?? throw new ArgumentNullException(nameof(fooRepository));
            _fooIdGenerator = fooIdGenerator ?? throw new ArgumentNullException(nameof(fooIdGenerator));
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Foo>> GetById(Guid id)
        {
            var foo = await _fooRepository.GetFooAsync(id);

            if (foo == null)
            {
                return NotFound();
            }

            return foo;
        }
        
        [HttpPost]
        public async Task<ActionResult<Foo>> Create([FromBody] CreateFooRequest request)
        {
            var foo = new Foo
            {
                Id = _fooIdGenerator.Create(),
                Name = request.Name
            };
            
            var inserted = await _fooRepository.TryInsertFooAsync(foo);

            if (!inserted)
            {
                return UnprocessableEntity();
            }

            return CreatedAtAction(nameof(GetById), new { id = foo.Id }, foo);
        }
    }
}