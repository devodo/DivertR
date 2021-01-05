namespace NMorph
{
    public interface IInvocationContext<T> where T : class
    {
        T Previous { get; }
        T Origin { get; }
    }
}