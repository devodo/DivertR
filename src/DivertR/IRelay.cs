using System.Reflection;

namespace DivertR
{
    public interface IRelay
    {
        object Next { get; }
        object Root { get; }
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
        new TTarget Next { get; }
        new TTarget Root { get; }
        new IRedirectCall<TTarget> GetCurrentCall();
    }
}