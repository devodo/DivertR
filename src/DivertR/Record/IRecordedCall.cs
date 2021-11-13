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
}
