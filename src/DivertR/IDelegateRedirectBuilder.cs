using System;

namespace DivertR
{
    public interface IDelegateRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        Redirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IDelegateRedirectBuilder<TTarget> Redirect(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
