using System.Reflection;

namespace DivertR
{
    public interface IRelay<TTarget> where TTarget : class
    {
        TTarget Next { get; }
        TTarget Original { get; }
        IRedirectCall<TTarget> GetCurrentCall();
        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments args);
        object? CallNext(CallArguments args);
        object? CallOriginal();
        object? CallOriginal(MethodInfo method, CallArguments args);
        object? CallOriginal(CallArguments args);
    }

    public interface IRelay<TTarget, out TReturn> : IRelay<TTarget> where TTarget : class
    {
        new TReturn CallNext();
        new TReturn CallNext(CallArguments args);
        new TReturn CallOriginal();
        new TReturn CallOriginal(CallArguments args);
    }
}