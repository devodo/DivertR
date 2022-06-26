using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirect Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(ICallHandler<TTarget> callHandler, IRedirectOptions<TTarget> redirectOptions);
        IRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
    
    public interface IRedirectBuilder
    {
        IRedirectBuilder AddConstraint(ICallConstraint callConstraint);
        IRedirect Build(object target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions);
        IRecordRedirect Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
