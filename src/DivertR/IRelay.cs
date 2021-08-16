using System.Reflection;
using DivertR.Core;

namespace DivertR
{
    public interface IRelay<TTarget> where TTarget : class
    {
        TTarget Next { get; }
        TTarget Original { get; }
        Redirect<TTarget> Redirect { get; }
        CallInfo<TTarget> CallInfo { get; }
        object? CallNext();
        object? CallNext(MethodInfo method, CallArguments callArguments);
        object? CallOriginal();
        object? CallOriginal(MethodInfo method, CallArguments callArguments);
    }
}