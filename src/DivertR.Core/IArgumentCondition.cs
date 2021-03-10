namespace DivertR.Core
{
    public interface IArgumentCondition
    {
        bool IsMatch(object? argument);
    }
}