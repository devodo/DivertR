using System.ComponentModel.DataAnnotations;

namespace DivertR.SampleWebApp.Rest
{
    public class CreateBarRequest
    {
        [Required]
        public string? Label { get; init; }
    }
}