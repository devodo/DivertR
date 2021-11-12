namespace DivertR.Record
{
    public interface IActionRecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
    }
    
    public interface IActionRecordedCall<TTarget, out TArgs> : IActionRecordedCall<TTarget>, IRecordedCall<TTarget, TArgs>
        where TTarget : class
    {
    }
}
