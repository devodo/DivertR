using System;

namespace DivertR.SampleWebApp.Model
{
    public record FooEvent
    {
        public Guid? EventId { get; init; }

        public FooEventType? EventType { get; init; }
        
        public Foo? Foo { get; init; }
    }
}