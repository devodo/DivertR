namespace DivertR
{
    public interface IRelay<out T> where T : class
    {
        T Next { get; }
        T Original { get; }
        T? OriginalInstance { get; }
        object? State { get; }
    }
}