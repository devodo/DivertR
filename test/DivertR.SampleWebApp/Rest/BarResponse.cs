using System;

namespace DivertR.SampleWebApp.Rest
{
    public class BarResponse
    {
        public required Guid Id { get; init; }

        public required string Label { get; init; }
        
        public required DateTime CreatedDate { get; init; }
    }
}