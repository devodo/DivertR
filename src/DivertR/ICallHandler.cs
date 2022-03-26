namespace DivertR
{
    public interface ICallHandler
    {
        object? Call(CallInfo callInfo);
    }
}