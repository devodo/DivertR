namespace DivertR.Record
{
    public interface IActionRecordedCall<TTarget> : IRecordedCall<TTarget> where TTarget : class
    {
    }
    
    public interface IActionRecordedCall<TTarget, out TArgs> : IActionRecordedCall<TTarget>
        where TTarget : class
    {
        new TArgs Args { get; }
    }
}
