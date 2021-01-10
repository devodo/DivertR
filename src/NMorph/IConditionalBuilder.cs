namespace NMorph
{
    public interface IConditionalBuilder<T, TReturn> where T : class
    {
        IAlterationBuilder<T> Retarget(T substitute);

        IAlterationBuilder<T> Return(TReturn value);
    }
}