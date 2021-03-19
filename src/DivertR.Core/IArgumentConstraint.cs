namespace DivertR.Core
{
    public interface IArgumentConstraint
    {
        bool IsMatch(object? argument);
    }
}