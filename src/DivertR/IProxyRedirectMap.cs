namespace DivertR
{
    public interface IProxyRedirectMap
    {
        void AddRedirect(IRedirect redirect, object proxy);
        IRedirect<TTarget> GetRedirect<TTarget>(TTarget proxy) where TTarget : class;
    }
}