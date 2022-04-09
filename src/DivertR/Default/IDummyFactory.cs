namespace DivertR.Default
{
    public interface IDummyFactory
    {
        TTarget Create<TTarget>() where TTarget : class;
    }
}