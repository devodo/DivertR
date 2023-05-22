using DivertR.WebApp.Model;
using DivertR.WebApp.Rest;
using DivertR.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DivertR.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BarController : ControllerBase
    {
        private readonly IBarServiceFactory _barServiceFactory;

        public BarController(IBarServiceFactory barServiceFactory)
        {
            _barServiceFactory = barServiceFactory ?? throw new ArgumentNullException(nameof(barServiceFactory));
        }

        [HttpPost("{name:regex(^\\w*$)}")]
        public async Task<ActionResult<BarResponse>> CreateBar(string name)
        {
            var barService = await _barServiceFactory.CreateBarService();
            var bar = await barService.CreateBarAsync(name);

            return MapBarResponse(bar);
        }
        
        private static BarResponse MapBarResponse(Bar bar)
        {
            return new BarResponse
            {
                Id = bar.Id,
                Label = bar.Label,
                CreatedDate = bar.CreatedDate
            };
        }
    }
}