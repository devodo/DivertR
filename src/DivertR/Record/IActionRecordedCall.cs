namespace DivertR.Record
{
    public interface IActionRecordedCall<TTarget> : IRecordedCall<TTarget>, IRecordedCallArgs where TTarget : class
    {
    }
}
