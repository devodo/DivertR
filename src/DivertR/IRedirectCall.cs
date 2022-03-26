namespace DivertR
{
    public interface IRedirectCall
    {
        CallInfo CallInfo { get; }
        CallArguments Args { get; }
        Redirect Redirect { get; }
    }
    
    public interface IRedirectCall<TTarget> : IRedirectCall where TTarget : class
    {
        new CallInfo<TTarget> CallInfo { get; }
    }
}