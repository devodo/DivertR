namespace DivertR.Record
{
    public interface IRecordedCall
    {
        ICallInfo CallInfo { get; }
        CallArguments Args { get; }
        ICallReturn? Returned { get; }
    }
    
    public interface IRecordedCall<out TTarget> : IRecordedCall where TTarget : class
    {
        new ICallInfo<TTarget> CallInfo { get; }
    }
    
    public interface IRecordedCall<out TTarget, out TArgs> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new TArgs Args { get; }
    }
}
