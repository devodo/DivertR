namespace DivertR.Record
{
    public interface IFuncRecordedCall<TTarget, out TReturn> : IRecordedCall<TTarget, TReturn>, IRecordedCallArgs where TTarget : class
    {
    }
}
