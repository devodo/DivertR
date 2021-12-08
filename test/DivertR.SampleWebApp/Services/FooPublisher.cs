using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;
using Microsoft.Extensions.Logging;

namespace DivertR.SampleWebApp.Services
{
    public class FooPublisher : IFooPublisher
    {
        private readonly ILogger<FooPublisher> _logger;

        public FooPublisher(ILogger<FooPublisher> logger)
        {
            _logger = logger;
        }
        
        public Task PublishAsync(FooEvent fooEvent)
        {
            _logger.LogInformation("Publishing foo event {EventId}", fooEvent.EventId);

            return Task.CompletedTask;
        }
    }
}