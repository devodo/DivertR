namespace DivertR
{
    public interface IProxyViaMap
    {
        void AddVia(IVia via, object proxy);
        IVia<TTarget> GetVia<TTarget>(TTarget proxy) where TTarget : class;
    }
}