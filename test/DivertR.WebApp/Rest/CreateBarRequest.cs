using System.ComponentModel.DataAnnotations;

namespace DivertR.WebApp.Rest
{
    public class CreateBarRequest
    {
        [Required]
        public string? Label { get; init; }
    }
}