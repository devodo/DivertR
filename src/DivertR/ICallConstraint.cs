namespace DivertR
{
    public interface ICallConstraint
    {
        bool IsMatch(CallInfo callInfo);
    }
}