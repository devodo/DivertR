namespace DivertR.Internal
{
    internal interface IArgumentConstraint
    {
        bool IsMatch(object? argument);
    }
}