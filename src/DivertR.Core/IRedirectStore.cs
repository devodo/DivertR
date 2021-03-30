namespace DivertR.Core
{
    public interface IRedirectStore<TTarget, TReturn> where TTarget : class
    {
        TReturn InsertRedirect(IRedirect<TTarget> redirect, int orderWeight = 0);
    }
}