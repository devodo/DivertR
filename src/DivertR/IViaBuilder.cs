using System;

namespace DivertR
{
    public interface IViaBuilder
    {
        IViaBuilder Filter(ICallConstraint callConstraint);
        IVia Build(Func<object?> viaDelegate);
        IVia Build(Func<IRedirectCall, object?> viaDelegate);
        IVia Build<TTarget>(Func<IRedirectCall<TTarget>, object?> viaDelegate) where TTarget : class?;
        IVia Build(ICallHandler callHandler);
    }
}