namespace Divertr
{
    public interface ICallContext<out T> where T : class
    {
        T Next { get; }
        T Root { get; }
    }
}