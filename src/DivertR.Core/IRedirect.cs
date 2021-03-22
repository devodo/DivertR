namespace DivertR.Core
{
    public interface IRedirect<out T> where T : class
    {
        object? Invoke(ICall call);
        bool IsMatch(ICall call);
    }
}