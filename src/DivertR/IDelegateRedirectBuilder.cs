using System;

namespace DivertR
{
    public interface IDelegateRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirect<TTarget> Build(Delegate redirectDelegate);
        IVia<TTarget> Redirect(Delegate redirectDelegate, int orderWeight = 0);
    }
}
