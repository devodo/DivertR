using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DivertR.SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase
    {
        private readonly IFooRepository _fooRepository;
        private readonly ILogger<FooController> _logger;

        public FooController(IFooRepository fooRepository, ILogger<FooController> logger)
        {
            _fooRepository = fooRepository;
            _logger = logger;
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Foo>> GetById(Guid id)
        {
            var foo = await _fooRepository.GetFoo(id);

            if (foo == null)
            {
                return NotFound();
            }

            return foo;
        }
        
        [HttpPost]
        public async Task<ActionResult<Foo>> Insert([FromBody]CreateFooRequest request)
        {
            var foo = new Foo
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };
            
            var inserted = await _fooRepository.TryInsertFoo(foo);

            if (!inserted)
            {
                return UnprocessableEntity();
            }
            
            return CreatedAtAction(nameof(GetById), new { id = foo.Id }, foo);
        }
    }
}