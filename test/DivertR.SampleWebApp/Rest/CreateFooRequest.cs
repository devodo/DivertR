using System;
using System.ComponentModel.DataAnnotations;

namespace DivertR.SampleWebApp.Rest
{
    public class CreateFooRequest
    {
        [Required]
        public Guid? Id { get; init; }
        
        [Required]
        public string? Name { get; init; }
    }
}