using System;

namespace DivertR
{
    public interface IRedirectOptions
    {
        int? OrderWeight { get; }
        bool? DisableSatisfyStrict { get; }
    }
    
    public interface IRedirectOptions<TTarget> : IRedirectOptions where TTarget : class
    {
        Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; }
        Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; }
    }
}