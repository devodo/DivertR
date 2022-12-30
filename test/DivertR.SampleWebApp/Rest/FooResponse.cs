using System;

namespace DivertR.SampleWebApp.Rest
{
    public class FooResponse
    {
        public required Guid Id { get; init; }

        public required string Name { get; init; }
    }
}