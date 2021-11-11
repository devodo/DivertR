namespace DivertR.Record
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        CallArguments Args { get; }
        ICallReturn? Returned { get; }
    }
}
