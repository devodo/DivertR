using DivertR.Core;

namespace DivertR.Redirects
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        object? ReturnedObject { get; }
    }

    public interface IRecordedCall<TTarget, TReturn> : IRecordedCall<TTarget> where TTarget : class
    {
        public TReturn Returned { get; }
    }
    
    public interface IRecordedCall<TTarget, TReturn, T1> : IRecordedCall<TTarget, TReturn> where TTarget : class
    {
        public T1 Arg1 { get; }
    }
    
    public interface IRecordedCall<TTarget, TReturn, T1, T2> : IRecordedCall<TTarget, TReturn, T1> where TTarget : class
    {
        public T2 Arg2 { get; }
    }
}