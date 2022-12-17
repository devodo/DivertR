using System;
using DivertR.Record;

namespace DivertR
{
    public interface IViaBuilder<TTarget> where TTarget : class?
    {
        IViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        IVia Build(object? instance);
        IVia Build(Func<object?> viaDelegate);
        IVia Build(Func<IRedirectCall<TTarget>, object?> viaDelegate);
        IVia Build(Func<IRedirectCall<TTarget>, CallArguments, object?> viaDelegate);
        IVia Build(ICallHandler<TTarget> callHandler);
        IRecordVia<TTarget> Record();
    }
    
    public interface IViaBuilder
    {
        IViaBuilder Filter(ICallConstraint callConstraint);
        IVia Build(object? instance);
        IVia Build(Func<object?> viaDelegate);
        IVia Build(Func<IRedirectCall, object?> viaDelegate);
        IVia Build(Func<IRedirectCall, CallArguments, object?> viaDelegate);
        IVia Build(ICallHandler callHandler);
        IRecordVia Record();
    }
}
