namespace DivertR
{
    public interface ICallConstraint<TTarget> where TTarget : class
    {
        bool IsMatch(CallInfo<TTarget> callInfo);
    }
}