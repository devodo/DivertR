using System.Reflection;

namespace DivertR
{
    public interface IRelay<TTarget> where TTarget : class
    {
        TTarget Next { get; }
        TTarget Root { get; }
        IRedirectCall<TTarget> GetCurrentCall();
        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments args);
        object? CallNext(CallArguments args);
        object? CallRoot();
        object? CallRoot(MethodInfo method, CallArguments args);
        object? CallRoot(CallArguments args);
    }

    public interface IRelay<TTarget, out TReturn> : IRelay<TTarget> where TTarget : class
    {
        new TReturn CallNext();
        new TReturn CallNext(CallArguments args);
        new TReturn CallRoot();
        new TReturn CallRoot(CallArguments args);
    }
}