using System;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect<TTarget> Build(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget> redirectOptions);
    }
    
    public interface IRedirectBuilder
    {
        IRedirectBuilder AddConstraint(ICallConstraint callConstraint);
        IRedirect Build(object target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions);
    }
}
