namespace DivertR.Core
{
    public interface ICallCondition
    {
        bool IsMatch(ICall call);
    }
}