namespace DivertR.Core
{
    public interface IRecordedCall<T> where T : class
    {
        CallInfo<T> CallInfo { get; }
        object? ReturnValue { get; set; }
    }
}