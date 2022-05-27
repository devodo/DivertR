namespace DivertR.Record
{
    public interface IFuncRecordedCall<TTarget, out TReturn> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new ICallReturn<TReturn>? Returned { get; }
    }
    
    public interface IFuncRecordedCall<TTarget, out TArgs, out TReturn> : IRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
        new ICallReturn<TReturn>? Returned { get; }
    }
}
