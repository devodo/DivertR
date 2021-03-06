namespace DivertR.Internal
{
    public interface ICallCondition
    {
        bool IsMatch(ICall call);
    }
}