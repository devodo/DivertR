namespace DivertR.Core
{
    public interface IRedirect<out T> where T : class
    {
        object? State { get; }
        object? Invoke(ICall call);
        bool IsMatch(ICall call);
    }
}