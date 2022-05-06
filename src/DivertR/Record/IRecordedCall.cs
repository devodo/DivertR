namespace DivertR.Record
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        ICallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        ICallReturn? Returned { get; }
    }
    
    public interface IRecordedCall<TTarget, out TArgs> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new TArgs Args { get; }
    }
}
