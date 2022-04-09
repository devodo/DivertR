using System.Reflection;

namespace DivertR
{
    public interface IRelay
    {
        IRedirectCall GetCurrentCall();
        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments args);
        object? CallNext(CallArguments args);
        object? CallRoot();
        object? CallRoot(MethodInfo method, CallArguments args);
        object? CallRoot(CallArguments args);
    }
    
    public interface IRelay<TTarget> : IRelay where TTarget : class
    {
        TTarget Next { get; }
        TTarget Root { get; }
        new IRedirectCall<TTarget> GetCurrentCall();
    }
}