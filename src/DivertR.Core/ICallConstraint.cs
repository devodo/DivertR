namespace DivertR.Core
{
    public interface ICallConstraint
    {
        bool IsMatch(CallInfo callInfo);
    }
}