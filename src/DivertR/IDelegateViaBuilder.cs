using System;

namespace DivertR
{
    public interface IDelegateViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        IRedirect<TTarget> Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IDelegateViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
