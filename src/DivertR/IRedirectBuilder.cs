using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class?
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirect Build(object? instance, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<IRedirectCall<TTarget>, object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(ICallHandler<TTarget> callHandler, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRedirect Build(ICallHandler<TTarget> callHandler, IRedirectOptions redirectOptions);
        IRecordRedirect<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
    
    public interface IRedirectBuilder
    {
        IRedirectBuilder AddConstraint(ICallConstraint callConstraint);
        IRedirect Build(object? instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<IRedirectCall, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(Func<IRedirectCall, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRedirect Build(ICallHandler callHandler, IRedirectOptions redirectOptions);
        IRecordRedirect Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
