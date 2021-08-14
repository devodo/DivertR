using System;

namespace DivertR
{
    public interface IDelegateRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        Redirect<TTarget> Build(Delegate redirectDelegate);
        IVia<TTarget> Redirect(Delegate redirectDelegate);
    }
}
