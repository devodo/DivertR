namespace DivertR.Record
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        ICallReturn? Returned { get; }
        CallArguments Args { get; }
    }
    
    public interface IRecordedCall<TTarget, out TArgs> : IRecordedCall<TTarget>
        where TTarget : class
    {
        new TArgs Args { get; }
    }
}
