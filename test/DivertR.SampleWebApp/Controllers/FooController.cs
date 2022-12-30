using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using DivertR.SampleWebApp.Rest;
using DivertR.SampleWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DivertR.SampleWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FooController : ControllerBase
    {
        private readonly IFooService _fooService;

        public FooController(IFooService fooService)
        {
            _fooService = fooService ?? throw new ArgumentNullException(nameof(fooService));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FooResponse>> GetById(Guid id)
        {
            var foo = await _fooService.GetFooAsync(id);

            if (foo == null)
            {
                return NotFound();
            }

            return MapFooResponse(foo);
        }
        
        [HttpPost]
        public async Task<ActionResult<FooResponse>> Create([FromBody] CreateFooRequest request)
        {
            try
            {
                var createdFoo = await _fooService.CreateFooAsync(request.Id!.Value, request.Name!);
                
                return CreatedAtAction(nameof(GetById), new { id = createdFoo.Id }, MapFooResponse(createdFoo));
            }
            catch (DuplicateFooException duplicateException)
            {
                return new ConflictObjectResult(MapFooResponse(duplicateException.Foo));
            }
        }

        private static FooResponse MapFooResponse(Foo foo)
        {
            return new FooResponse
            {
                Id = foo.Id,
                Name = foo.Name
            };
        }
    }
}