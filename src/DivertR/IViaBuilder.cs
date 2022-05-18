using System;
using DivertR.Record;

namespace DivertR
{
    public interface IViaBuilder<TTarget> where TTarget : class
    {
        IViaBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirect<TTarget> Build(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IViaBuilder<TTarget> Retarget(TTarget target, Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
        IRecordStream<TTarget> Record(Action<IRedirectOptionsBuilder<TTarget>>? optionsAction = null);
    }
}
