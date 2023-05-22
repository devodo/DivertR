using System.ComponentModel.DataAnnotations;

namespace DivertR.WebApp.Rest
{
    public class CreateFooRequest
    {
        [Required]
        public Guid? Id { get; init; }
        
        [Required]
        public string? Name { get; init; }
    }
}