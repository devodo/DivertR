using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint callConstraint);
        Redirect Build(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IVia<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder>? optionsAction = null);
    }
}
