using System;
using DivertR.Record;

namespace DivertR
{
    public interface IViaBuilder<TTarget> where TTarget : class?
    {
        IVia<TTarget> Via { get; }
        IRedirectBuilder<TTarget> RedirectBuilder { get; }
        
        IViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        IViaBuilder<TTarget> Redirect(object? instance, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IViaBuilder<TTarget> Redirect(Func<object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IViaBuilder<TTarget> Redirect(Func<IRedirectCall<TTarget>, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IViaBuilder<TTarget> Redirect(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
