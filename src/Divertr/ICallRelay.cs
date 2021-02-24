namespace Divertr
{
    public interface ICallRelay<out T> where T : class
    {
        T Next { get; }
        T Original { get; }
        object? State { get; }
    }
}