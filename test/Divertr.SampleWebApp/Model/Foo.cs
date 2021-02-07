using System;

namespace Divertr.SampleWebApp.Model
{
    public record Foo
    {
         public Guid Id { get; init; }

         public string Name { get; init; }
    }
}