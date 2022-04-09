namespace DivertR.Default
{
    public interface IDummyVia : IVia
    {
        TTarget Proxy<TTarget>(TTarget? root = null) where TTarget : class;
    }
}