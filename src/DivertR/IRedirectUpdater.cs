using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectUpdater<TTarget> where TTarget : class?
    {
        IRedirectUpdater<TTarget> Filter(ICallConstraint callConstraint);
        IRedirectUpdater<TTarget> Via(ICallHandler callHandler, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Via(Func<object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Via(Func<IRedirectCall<TTarget>, object?> viaDelegate, Action<IViaOptionsBuilder>? optionsAction = null);
        IRedirectUpdater<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IViaOptionsBuilder>? optionsAction = null);
    }
}
