using System.Threading.Tasks;
using Divertr.SampleWebApp.Model;
using Microsoft.Extensions.Logging;

namespace Divertr.SampleWebApp.Services
{
    public class FooPublisher : IFooPublisher
    {
        private readonly ILogger<FooPublisher> _logger;

        public FooPublisher(ILogger<FooPublisher> logger)
        {
            _logger = logger;
        }
        
        public Task Publish(FooEvent fooEvent)
        {
            _logger.LogInformation("Publishing foo event {EventId}", fooEvent.EventId);

            return Task.CompletedTask;
        }
    }
}