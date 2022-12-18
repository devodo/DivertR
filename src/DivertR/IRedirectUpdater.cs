using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectUpdater<TTarget> where TTarget : class?
    {
        IRedirect<TTarget> Redirect { get; }
        IViaBuilder<TTarget> ViaBuilder { get; }
        
        IRedirectUpdater<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        IRedirectUpdater<TTarget> Via(object? instance, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, CallArguments, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
