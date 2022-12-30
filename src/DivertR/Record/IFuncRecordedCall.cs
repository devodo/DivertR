namespace DivertR.Record
{
    public interface IFuncRecordedCall<out TTarget, out TReturn> : IRecordedCall<TTarget>
        where TTarget : class?
    {
        new TReturn? Return { get; }
        new TReturn? ReturnOrDefault { get; }
    }
    
    public interface IFuncRecordedCall<out TTarget, out TReturn, out TArgs> : IRecordedCall<TTarget, TArgs>
        where TTarget : class?
    {
        new TReturn? Return { get; }
        new TReturn? ReturnOrDefault { get; }
    }
    
    public interface IFuncRecordedCall<out TReturn> : IRecordedCall
    {
        new TReturn? Return { get; }
        new TReturn? ReturnOrDefault { get; }
    }
}
