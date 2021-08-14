using DivertR.Core;

namespace DivertR
{
    public interface IRelay<TTarget> where TTarget : class
    {
        TTarget Next { get; }
        TTarget Original { get; }
        Redirect<TTarget> Redirect { get; }
        CallInfo<TTarget> CallInfo { get; }
        object? CallNext(CallInfo<TTarget>? callInfo = null);
        object? CallOriginal(CallInfo<TTarget>? callInfo = null);
    }
}