using System.Reflection;

namespace DivertR
{
    public interface IRedirectCall
    {
        IRelay Relay { get; }
        CallInfo CallInfo { get; }
        CallArguments Args { get; }
        Redirect Redirect { get; }
        object Next { get; }
        object Root { get; }
        
        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments args);
        object? CallNext(CallArguments args);
        object? CallRoot();
        object? CallRoot(MethodInfo method, CallArguments args);
        object? CallRoot(CallArguments args);
    }
    
    public interface IRedirectCall<TTarget> : IRedirectCall where TTarget : class
    {
        new CallInfo<TTarget> CallInfo { get; }
        new IRelay<TTarget> Relay { get; }
        new TTarget Next { get; }
        new TTarget Root { get; }
    }
}