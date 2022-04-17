namespace DivertR
{
    public interface IBaseCallConstraint<in TCallInfo>
        where TCallInfo : CallInfo
    {
        bool IsMatch(TCallInfo callInfo);
    }

    public interface ICallConstraint : IBaseCallConstraint<CallInfo>
    {
    }
    
    public interface ICallConstraint<TTarget> : IBaseCallConstraint<CallInfo<TTarget>> where TTarget : class
    {
    }
}