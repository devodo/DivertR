namespace DivertR.Core
{
    public interface ICallConstraint
    {
        bool IsMatch(ICall call);
    }
}