namespace Divertr
{
    public interface ICallContext<T> where T : class
    {
        T Replaced { get; }
        T Original { get; }
    }
}