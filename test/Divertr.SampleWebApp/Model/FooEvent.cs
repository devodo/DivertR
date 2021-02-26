using System;

namespace DivertR.SampleWebApp.Model
{
    public record FooEvent
    {
        public Guid EventId { get; init; }

        public FooEventType EventType { get; set; }
        
        public Foo Foo { get; init; }
    }
}