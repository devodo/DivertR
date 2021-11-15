using System;

namespace DivertR
{
    public interface IDelegateRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        Redirect<TTarget> Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IVia<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
