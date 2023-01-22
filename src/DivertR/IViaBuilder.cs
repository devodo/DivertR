using System;

namespace DivertR
{
    public interface IViaBuilder<TTarget> where TTarget : class?
    {
        IViaBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        IVia Build(Func<object?> viaDelegate);
        IVia Build(Func<IRedirectCall<TTarget>, object?> viaDelegate);
        IVia Build(Func<IRedirectCall<TTarget>, CallArguments, object?> viaDelegate);
        IVia Build(ICallHandler<TTarget> callHandler);
    }
    
    public interface IViaBuilder
    {
        IViaBuilder Filter(ICallConstraint callConstraint);
        IVia Build(Func<object?> viaDelegate);
        IVia Build(Func<IRedirectCall, object?> viaDelegate);
        IVia Build(Func<IRedirectCall, CallArguments, object?> viaDelegate);
        IVia Build(ICallHandler callHandler);
    }
}