namespace NMorph
{
    public interface IMorphInvocation<T> where T : class
    {
        T ReplacedTarget { get; }
        T OriginalTarget { get; }
    }
}