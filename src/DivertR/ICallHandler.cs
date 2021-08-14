using DivertR.Core;

namespace DivertR
{
    public interface ICallHandler<TTarget> where TTarget : class
    {
        object? Call(CallInfo<TTarget> callInfo);
    }
}