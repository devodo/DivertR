using Microsoft.AspNetCore.Mvc;

namespace DivertR.SampleWebApp.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}