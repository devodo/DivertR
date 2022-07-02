using System.Reflection;

namespace DivertR
{
    public interface IRedirectCall
    {
        IRelay Relay { get; }
        ICallInfo CallInfo { get; }
        CallArguments Args { get; }

        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments args);
        object? CallNext(CallArguments args);
        object? CallRoot();
        object? CallRoot(MethodInfo method, CallArguments args);
        object? CallRoot(CallArguments args);
    }
    
    public interface IRedirectCall<TTarget> : IRedirectCall where TTarget : class
    {
        new ICallInfo<TTarget> CallInfo { get; }
        new IRelay<TTarget> Relay { get; }
        TTarget Next { get; }
        TTarget Root { get; }
    }
}