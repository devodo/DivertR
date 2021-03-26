namespace DivertR.Core
{
    public interface ICallConstraint<T> where T : class
    {
        bool IsMatch(CallInfo<T> callInfo);
    }
}