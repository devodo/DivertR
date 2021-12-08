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
        private readonly IFooPublisher _fooPublisher;

        public FooController(IFooRepository fooRepository, IFooPublisher fooPublisher)
        {
            _fooRepository = fooRepository ?? throw new ArgumentNullException(nameof(fooRepository));
            _fooPublisher = fooPublisher ?? throw new ArgumentNullException(nameof(fooPublisher));
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
                Id = Guid.NewGuid(),
                Name = request.Name
            };
            
            var inserted = await _fooRepository.TryInsertFooAsync(foo);

            if (!inserted)
            {
                return UnprocessableEntity();
            }

            await _fooPublisher.PublishAsync(
                new FooEvent { EventId = Guid.NewGuid(), EventType = FooEventType.Created, Foo = foo });
            
            return CreatedAtAction(nameof(GetById), new { id = foo.Id }, foo);
        }
    }
}