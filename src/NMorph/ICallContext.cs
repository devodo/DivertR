namespace NMorph
{
    public interface ICallContext<T> where T : class
    {
        T Previous { get; }
        T Origin { get; }
    }
}