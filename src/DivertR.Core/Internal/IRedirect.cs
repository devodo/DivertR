namespace DivertR.Core.Internal
{
    internal interface IRedirect<out T> where T : class
    {
        T Target { get; }
        object? State { get; }
        bool IsMatch(ICall call);
    }
}