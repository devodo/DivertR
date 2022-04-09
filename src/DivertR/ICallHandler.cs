namespace DivertR
{
    public interface ICallHandler
    {
        object? Call(IRedirectCall call);
    }
}