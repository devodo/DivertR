namespace Divertr
{
    public interface ICallContext<T> where T : class
    {
        T Next { get; }
        T Root { get; }
    }
}