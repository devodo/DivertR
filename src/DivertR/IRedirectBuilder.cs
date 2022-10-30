using System;
using DivertR.Record;

namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class?
    {
        IRedirectBuilder<TTarget> Filter(ICallConstraint<TTarget> callConstraint);
        IRedirect Build(object? instance);
        IRedirect Build(Func<object?> redirectDelegate);
        IRedirect Build(Func<IRedirectCall<TTarget>, object?> redirectDelegate);
        IRedirect Build(Func<IRedirectCall<TTarget>, CallArguments, object?> redirectDelegate);
        IRedirect Build(ICallHandler<TTarget> callHandler);
        IRecordRedirect<TTarget> Record();
    }
    
    public interface IRedirectBuilder
    {
        IRedirectBuilder Filter(ICallConstraint callConstraint);
        IRedirect Build(object? instance);
        IRedirect Build(Func<object?> redirectDelegate);
        IRedirect Build(Func<IRedirectCall, object?> redirectDelegate);
        IRedirect Build(Func<IRedirectCall, CallArguments, object?> redirectDelegate);
        IRedirect Build(ICallHandler callHandler);
        IRecordRedirect Record();
    }
}
