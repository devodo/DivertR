using System;

namespace DivertR
{
    public interface IDelegateRedirectBuilder<TTarget> : IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirect<TTarget> Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
    
    public interface IDelegateRedirectBuilder : IRedirectBuilder
    {
        IRedirect Build(Delegate redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
