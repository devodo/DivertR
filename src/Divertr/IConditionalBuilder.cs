namespace Divertr
{
    public interface IConditionalBuilder<T, TReturn> where T : class
    {
        IDiversionBuilder<T> SendTo(T substitute);

        IDiversionBuilder<T> Return(TReturn value);
    }
}