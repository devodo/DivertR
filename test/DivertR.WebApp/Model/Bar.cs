namespace DivertR.WebApp.Model
{
    public class Bar
    {
        public required Guid Id { get; init; }
        
        public required string Label { get; init; }

        public required DateTime CreatedDate { get; init; }
    }
}