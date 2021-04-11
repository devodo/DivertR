using DivertR.Core;

namespace DivertR.Redirects
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        object? ReturnValue { get; set; }
    }
}