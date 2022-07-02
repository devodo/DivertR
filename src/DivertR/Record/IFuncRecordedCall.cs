namespace DivertR.Record
{
    public interface IFuncRecordedCall<out TTarget, out TReturn> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new ICallReturn<TReturn>? Returned { get; }
    }
    
    public interface IFuncRecordedCall<out TTarget, out TReturn, out TArgs> : IRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
        new ICallReturn<TReturn>? Returned { get; }
    }
    
    public interface IFuncRecordedCall<out TReturn> : IRecordedCall
    {
        new ICallReturn<TReturn>? Returned { get; }
    }
}
