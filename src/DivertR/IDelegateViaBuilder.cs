using System;

namespace DivertR
{
    public interface IDelegateViaBuilder<TTarget> : IViaBuilder<TTarget> where TTarget : class
    {
        new IDelegateRedirectBuilder<TTarget> RedirectBuilder { get; }
        
        IDelegateViaBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
