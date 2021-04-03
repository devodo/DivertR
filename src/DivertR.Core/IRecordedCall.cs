namespace DivertR.Core
{
    public interface IRecordedCall<TTarget> where TTarget : class
    {
        CallInfo<TTarget> CallInfo { get; }
        object? ReturnValue { get; set; }
    }
}