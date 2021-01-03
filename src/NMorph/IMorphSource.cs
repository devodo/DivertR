namespace NMorph
{
    public interface IMorphSource<T> where T : class
    {
        T ReplacedTarget { get; }
        IInvocationContext<T> Invocation { get; }
    }
}