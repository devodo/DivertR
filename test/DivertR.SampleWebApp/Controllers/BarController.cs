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