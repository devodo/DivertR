using System;
using DivertR.Record;

namespace DivertR
{
    public interface IViaBuilder<TTarget> where TTarget : class
    {
        IVia<TTarget> Via { get; }
        IRedirectBuilder<TTarget> RedirectBuilder { get; }
        
        IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
