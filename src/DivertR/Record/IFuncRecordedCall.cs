namespace DivertR.Record
{
    public interface IFuncRecordedCall<TTarget, out TReturn> : IRecordedCall<TTarget> where TTarget : class
    {
        public new ICallReturn<TReturn>? Returned { get; }
    }
    
    public interface IFuncRecordedCall<TTarget, out TReturn, out TArgs> : IFuncRecordedCall<TTarget, TReturn>, IRecordedCallArgs where TTarget : class
    {
    }
}
