namespace DivertR.Record
{
    public interface IRecordedCall
    {
        CallArguments Args { get; }
        ICallReturn? Returned { get; }
    }
    
    public interface IRecordedCall<TTarget> : IRecordedCall where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
    }
    
    public interface IRecordedCall<TTarget, out TArgs> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new TArgs Args { get; }
    }
}
