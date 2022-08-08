using System;

namespace DivertR.SampleWebApp.Model
{
    public record Foo
    {
        public Guid Id { get; init; }

        public string? Name { get; init; }
    }
}