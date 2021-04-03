namespace DivertR.Core
{
    public interface ICallConstraint<TTarget> where TTarget : class
    {
        bool IsMatch(CallInfo<TTarget> callInfo);
    }
}