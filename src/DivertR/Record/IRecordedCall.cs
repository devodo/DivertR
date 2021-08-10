using DivertR.Core;

namespace DivertR.Record
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        ICallReturn? Returned { get; }
    }

    public interface IRecordedCall<TTarget, out TReturn> : IRecordedCall<TTarget> where TTarget : class
    {
        public new ICallReturn<TReturn>? Returned { get; }
    }
    
    public interface IRecordedCall<TTarget, out TReturn, out T1> : IRecordedCall<TTarget, TReturn> where TTarget : class
    {
        public T1 Arg1 { get; }
    }
    
    public interface IRecordedCall<TTarget, out TReturn, out T1, out T2> : IRecordedCall<TTarget, TReturn, T1> where TTarget : class
    {
        public T2 Arg2 { get; }
    }
    
    public interface IRecordedCall<TTarget, out TReturn, out T1, out T2, out T3> : IRecordedCall<TTarget, TReturn, T1, T2> where TTarget : class
    {
        public T3 Arg3 { get; }
    }
    
    public interface IRecordedCall<TTarget, out TReturn, out T1, out T2, out T3, out T4> : IRecordedCall<TTarget, TReturn, T1, T2, T3> where TTarget : class
    {
        public T4 Arg4 { get; }
    }
}
