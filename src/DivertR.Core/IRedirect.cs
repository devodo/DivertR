namespace DivertR.Core
{
    public interface IRedirect<out T> where T : class
    {
        object? Call(CallInfo callInfo);
        bool IsMatch(CallInfo callInfo);
    }
}