using DivertR.Core;

namespace DivertR.Record
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        ICallReturn? Returned { get; }
    }

    public interface IRecordedCall<TTarget, out TReturn> : IRecordedCall<TTarget> where TTarget : class
    {
        public new ICallReturn<TReturn>? Returned { get; }
    }
}
